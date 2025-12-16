using CatalogService.Application.Contracts.Persistence;
using CatalogService.Infrastructure.Messaging.Events;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace CatalogService.Infrastructure.Messaging;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMQConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "rabbitmq",
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(exchange: "library_events", type: ExchangeType.Topic, durable: true, cancellationToken: cancellationToken);

        var queueDeclareResult = await _channel.QueueDeclareAsync(cancellationToken: cancellationToken);
        var queueName = queueDeclareResult.QueueName;
        await _channel.QueueBindAsync(queue: queueName, exchange: "library_events", routingKey: "book.borrowed", cancellationToken: cancellationToken);
        await _channel.QueueBindAsync(queue: queueName, exchange: "library_events", routingKey: "book.returned", cancellationToken: cancellationToken);

        Console.WriteLine("[Catalog] RabbitMQ consumer started");

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel == null) return;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICatalogRepository>();

            try
            {
                if (routingKey == "book.borrowed")
                {
                    var bookBorrowed = JsonSerializer.Deserialize<BookBorrowedEvent>(message);
                    if (bookBorrowed != null)
                    {
                        var book = await repository.GetByIdAsync(bookBorrowed.BookId);
                        if (book != null)
                        {
                            book.IsAvailable = false;
                            book.UpdatedAt = DateTime.UtcNow;
                            await repository.UpdateAsync(book);
                            Console.WriteLine($"[Catalog] Book {book.Id} marked as unavailable");
                        }
                    }
                }
                else if (routingKey == "book.returned")
                {
                    var bookReturned = JsonSerializer.Deserialize<BookReturnedEvent>(message);
                    if (bookReturned != null)
                    {
                        var book = await repository.GetByIdAsync(bookReturned.BookId);
                        if (book != null)
                        {
                            book.IsAvailable = true;
                            book.UpdatedAt = DateTime.UtcNow;
                            await repository.UpdateAsync(book);
                            Console.WriteLine($"[Catalog] Book {book.Id} marked as available");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Catalog] Error processing message: {ex.Message}");
            }
        };

        var queueDeclareResult = await _channel.QueueDeclareAsync(cancellationToken: stoppingToken);
        await _channel.BasicConsumeAsync(queue: queueDeclareResult.QueueName, autoAck: true, consumer: consumer, cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async void Dispose()
    {
        if (_channel != null)
            await _channel.CloseAsync();
        if (_connection != null)
            await _connection.CloseAsync();
        base.Dispose();
    }
}
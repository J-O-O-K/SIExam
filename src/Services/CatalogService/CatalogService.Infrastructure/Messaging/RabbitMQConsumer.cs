using CatalogService.Application.Contracts.Persistence;
using CatalogService.Infrastructure.Messaging.Events;
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
    private const string QueueName = "catalog_service_queue";

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

        await _channel.ExchangeDeclareAsync(
            exchange: "library_events", 
            type: ExchangeType.Topic, 
            durable: true, 
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,      
            exclusive: false,    
            autoDelete: false,   
            arguments: null,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            queue: QueueName, 
            exchange: "library_events", 
            routingKey: "book.borrowed", 
            cancellationToken: cancellationToken);
        await _channel.QueueBindAsync(
            queue: QueueName, 
            exchange: "library_events", 
            routingKey: "book.returned", 
            cancellationToken: cancellationToken);

        Console.WriteLine($"[Catalog] RabbitMQ consumer started, queue: {QueueName}");

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

            Console.WriteLine($"[Catalog] Received message with routing key: {routingKey}");

            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<ICatalogRepository>();

            try
            {
                if (routingKey == "book.borrowed")
                {
                    var bookBorrowed = JsonSerializer.Deserialize<BookBorrowedEvent>(message);
                    if (bookBorrowed != null)
                    {
                        Console.WriteLine($"[Catalog] Processing BookBorrowed event for BookId: {bookBorrowed.BookId}");
                        var book = await repository.GetByIdAsync(bookBorrowed.BookId);
                        if (book != null)
                        {
                            book.IsAvailable = false;
                            book.UpdatedAt = DateTime.UtcNow;
                            await repository.UpdateAsync(book);
                            Console.WriteLine($"[Catalog] Book {book.Id} marked as unavailable");
                        }
                        else
                        {
                            Console.WriteLine($"[Catalog] Book {bookBorrowed.BookId} not found");
                        }
                    }
                }
                else if (routingKey == "book.returned")
                {
                    var bookReturned = JsonSerializer.Deserialize<BookReturnedEvent>(message);
                    if (bookReturned != null)
                    {
                        Console.WriteLine($"[Catalog] Processing BookReturned event for BookId: {bookReturned.BookId}");
                        var book = await repository.GetByIdAsync(bookReturned.BookId);
                        if (book != null)
                        {
                            book.IsAvailable = true;
                            book.UpdatedAt = DateTime.UtcNow;
                            await repository.UpdateAsync(book);
                            Console.WriteLine($"[Catalog] Book {book.Id} marked as available");
                        }
                        else
                        {
                            Console.WriteLine($"[Catalog] Book {bookReturned.BookId} not found");
                        }
                    }
                }

                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Catalog] Error processing message: {ex.Message}");
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: QueueName, 
            autoAck: false,
            consumer: consumer, 
            cancellationToken: stoppingToken);

        Console.WriteLine($"[Catalog] Started consuming messages from queue: {QueueName}");

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
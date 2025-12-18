using System.Text;
using System.Text.Json;
using LendingService.Application.Contracts.Messaging;
using LendingService.Infrastructure.Messaging.Events;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace LendingService.Infrastructure.Messaging;

public class RabbitMQPublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMQPublisher(IConfiguration configuration)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:Host"] ?? "rabbitmq",
            UserName = configuration["RabbitMQ:Username"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest"
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync(
            exchange: "library_events",
            type: ExchangeType.Topic,
            durable: true).GetAwaiter().GetResult();
    }

    public void PublishBookBorrowed(int bookId, int userId, DateTime borrowedAt)
    {
        var eventMessage = new BookBorrowedEvent
        {
            BookId = bookId,
            UserId = userId,
            BorrowedAt = borrowedAt
        };

        var message = JsonSerializer.Serialize(eventMessage);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        _channel.BasicPublishAsync(
            exchange: "library_events",
            routingKey: "book.borrowed",
            mandatory: false,
            basicProperties: properties,
            body: body).GetAwaiter().GetResult();

        Console.WriteLine($"[Lending] Published BookBorrowed event for BookId: {bookId}");
    }

    public void PublishBookReturned(int bookId, int userId, DateTime returnedAt)
    {
        var eventMessage = new BookReturnedEvent
        {
            BookId = bookId,
            UserId = userId,
            ReturnedAt = returnedAt
        };

        var message = JsonSerializer.Serialize(eventMessage);
        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true  
        };

        _channel.BasicPublishAsync(
            exchange: "library_events",
            routingKey: "book.returned",
            mandatory: false,
            basicProperties: properties, 
            body: body).GetAwaiter().GetResult();

        Console.WriteLine($"[Lending] Published BookReturned event for BookId: {bookId}");
    }

    public void Dispose()
    {
        _channel?.CloseAsync().GetAwaiter().GetResult();
        _connection?.CloseAsync().GetAwaiter().GetResult();
    }
}
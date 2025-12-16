namespace CatalogService.Infrastructure.Messaging.Events;

public class BookReturnedEvent
{
    public int BookId { get; set; }
    public int UserId { get; set; }
    public DateTime ReturnedAt { get; set; }
}
namespace LendingService.Infrastructure.Messaging.Events;

public class BookBorrowedEvent
{
    public int BookId { get; set; }
    public int UserId { get; set; }
    public DateTime BorrowedAt { get; set; }
}
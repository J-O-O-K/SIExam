namespace LendingService.Application.Contracts.Messaging;

public interface IMessagePublisher
{
    void PublishBookBorrowed(int bookId, int userId, DateTime borrowedAt);
    void PublishBookReturned(int bookId, int userId, DateTime returnedAt);
}
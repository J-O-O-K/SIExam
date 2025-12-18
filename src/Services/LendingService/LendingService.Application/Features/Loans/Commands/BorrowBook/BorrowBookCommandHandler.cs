using AutoMapper;
using LendingService.Application.Contracts.Messaging;
using LendingService.Application.Contracts.Persistence;
using LendingService.Application.DTOs;
using LendingService.Domain.Entities;
using MediatR;

namespace LendingService.Application.Features.Loans.Commands.BorrowBook;

public class BorrowBookCommandHandler : IRequestHandler<BorrowBookCommand, LoanDto>
{
    private readonly ILendingRepository _repository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMapper _mapper;

    public BorrowBookCommandHandler(
        ILendingRepository repository,
        IMessagePublisher messagePublisher,
        IMapper mapper)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    public async Task<LoanDto> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
    {
        // Check if already borrowed
        var existingLoan = await _repository.GetActiveLoanByBookIdAsync(request.BookId);
        if (existingLoan != null)
        {
            throw new InvalidOperationException($"Book with ID {request.BookId} is already borrowed");
        }

        var loan = new Loan
        {
            BookId = request.BookId,
            UserId = request.UserId,
            Username = request.Username,
            BorrowedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            IsActive = true
        };

        var createdLoan = await _repository.CreateAsync(loan);

        // Publish to RabbitMQ
        _messagePublisher.PublishBookBorrowed(createdLoan.BookId, createdLoan.UserId, createdLoan.BorrowedAt);

        return _mapper.Map<LoanDto>(createdLoan);
    }
}
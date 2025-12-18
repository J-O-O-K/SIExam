using AutoMapper;
using LendingService.Application.Contracts.Messaging;
using LendingService.Application.Contracts.Persistence;
using LendingService.Application.DTOs;
using MediatR;

namespace LendingService.Application.Features.Loans.Commands.ReturnBook;

public class ReturnBookCommandHandler : IRequestHandler<ReturnBookCommand, LoanDto>
{
    private readonly ILendingRepository _repository;
    private readonly IMessagePublisher _messagePublisher;
    private readonly IMapper _mapper;

    public ReturnBookCommandHandler(
        ILendingRepository repository,
        IMessagePublisher messagePublisher,
        IMapper mapper)
    {
        _repository = repository;
        _messagePublisher = messagePublisher;
        _mapper = mapper;
    }

    public async Task<LoanDto> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var loan = await _repository.GetByIdAsync(request.LoanId);
        if (loan == null)
        {
            throw new KeyNotFoundException($"Loan with ID {request.LoanId} not found");
        }

        if (!loan.IsActive)
        {
            throw new InvalidOperationException($"Loan with ID {request.LoanId} is already returned");
        }

        loan.ReturnedAt = DateTime.UtcNow;
        loan.IsActive = false;

        var updatedLoan = await _repository.UpdateAsync(loan);

        // Publish event to RabbitMQ
        _messagePublisher.PublishBookReturned(updatedLoan.BookId, updatedLoan.UserId, updatedLoan.ReturnedAt!.Value);

        return _mapper.Map<LoanDto>(updatedLoan);
    }
}
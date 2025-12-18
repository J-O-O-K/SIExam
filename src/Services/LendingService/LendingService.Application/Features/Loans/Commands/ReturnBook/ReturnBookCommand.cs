using LendingService.Application.DTOs;
using MediatR;

namespace LendingService.Application.Features.Loans.Commands.ReturnBook;

public class ReturnBookCommand : IRequest<LoanDto>
{
    public int LoanId { get; set; }
}
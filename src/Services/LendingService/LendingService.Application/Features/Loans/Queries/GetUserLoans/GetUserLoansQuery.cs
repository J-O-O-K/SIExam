using LendingService.Application.DTOs;
using MediatR;

namespace LendingService.Application.Features.Loans.Queries.GetUserLoans;

public class GetUserLoansQuery : IRequest<IEnumerable<LoanDto>>
{
    public int UserId { get; set; }
}
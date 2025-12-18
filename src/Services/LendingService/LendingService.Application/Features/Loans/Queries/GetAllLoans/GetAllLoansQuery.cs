using LendingService.Application.DTOs;
using MediatR;

namespace LendingService.Application.Features.Loans.Queries.GetAllLoans;

public class GetAllLoansQuery : IRequest<IEnumerable<LoanDto>>
{
}
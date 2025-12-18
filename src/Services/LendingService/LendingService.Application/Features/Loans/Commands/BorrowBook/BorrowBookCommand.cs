using LendingService.Application.DTOs;
using MediatR;

namespace LendingService.Application.Features.Loans.Commands.BorrowBook;

public class BorrowBookCommand : IRequest<LoanDto>
{
    public int BookId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
}
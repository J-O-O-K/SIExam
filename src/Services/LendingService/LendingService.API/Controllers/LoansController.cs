using LendingService.Application.DTOs;
using LendingService.Application.DTOs.LoanDTOs;
using LendingService.Application.Features.Loans.Commands.BorrowBook;
using LendingService.Application.Features.Loans.Commands.ReturnBook;
using LendingService.Application.Features.Loans.Queries.GetAllLoans;
using LendingService.Application.Features.Loans.Queries.GetUserLoans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LendingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoansController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Get all loans
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAllLoans()
    {
        var query = new GetAllLoansQuery();
        var loans = await _mediator.Send(query);
        return Ok(loans);
    }

    // Get loans for logged in user
    [HttpGet("my-loans")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetMyLoans()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var query = new GetUserLoansQuery { UserId = userId };
        var loans = await _mediator.Send(query);
        return Ok(loans);
    }

    // Borrow book
    [HttpPost("borrow")]
    public async Task<ActionResult<LoanDto>> BorrowBook([FromBody] BorrowBookDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        try
        {
            var command = new BorrowBookCommand
            {
                BookId = dto.BookId,
                UserId = userId,
                Username = username ?? "Unknown"
            };

            var loan = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetMyLoans), null, loan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Return book
    [HttpPost("{loanId}/return")]
    public async Task<ActionResult<LoanDto>> ReturnBook(int loanId)
    {
        try
        {
            var command = new ReturnBookCommand { LoanId = loanId };
            var loan = await _mediator.Send(command);
            return Ok(loan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
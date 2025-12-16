using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using CatalogService.Application.Features.Books.Commands.CreateBook;
using CatalogService.Application.Features.Books.Commands.DeleteBook;
using CatalogService.Application.Features.Books.Commands.UpdateBook;
using CatalogService.Application.Features.Books.Queries.GetAllBooks;
using CatalogService.Application.Features.Books.Queries.GetBookById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Get all books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAllBooks()
    {
        var query = new GetAllBooksQuery();
        var books = await _mediator.Send(query);
        return Ok(books);
    }

    // Get a specific book by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        try
        {
            var query = new GetBookByIdQuery { Id = id };
            var book = await _mediator.Send(query);
            return Ok(book);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Create a new book
    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookCommand command)
    {
        try
        {
            var book = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Update an existing book
    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> UpdateBook(int id, [FromBody] UpdateBookDto dto)
    {
        try
        {
            var command = new UpdateBookCommand
            {
                Id = id,
                Title = dto.Title,
                Author = dto.Author
            };
            var book = await _mediator.Send(command);
            return Ok(book);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // Delete a book
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            var command = new DeleteBookCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
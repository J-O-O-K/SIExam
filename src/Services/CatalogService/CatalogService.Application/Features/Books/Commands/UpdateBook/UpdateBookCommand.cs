using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using MediatR;

namespace CatalogService.Application.Features.Books.Commands.UpdateBook;

public class UpdateBookCommand : IRequest<BookDto>
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
}
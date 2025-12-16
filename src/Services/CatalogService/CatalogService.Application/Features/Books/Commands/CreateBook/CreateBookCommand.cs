using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using MediatR;

namespace CatalogService.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommand : IRequest<BookDto>
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
}
using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using MediatR;

namespace CatalogService.Application.Features.Books.Queries.GetBookById;

public class GetBookByIdQuery : IRequest<BookDto>
{
    public int Id { get; set; }
}
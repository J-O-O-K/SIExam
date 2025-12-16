using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using MediatR;

namespace CatalogService.Application.Features.Books.Queries.GetAllBooks;

public class GetAllBooksQuery : IRequest<IEnumerable<BookDto>>
{
}
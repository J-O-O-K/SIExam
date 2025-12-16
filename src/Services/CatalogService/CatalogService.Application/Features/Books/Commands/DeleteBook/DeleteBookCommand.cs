using MediatR;

namespace CatalogService.Application.Features.Books.Commands.DeleteBook;

public class DeleteBookCommand : IRequest<Unit>
{
    public int Id { get; set; }
}
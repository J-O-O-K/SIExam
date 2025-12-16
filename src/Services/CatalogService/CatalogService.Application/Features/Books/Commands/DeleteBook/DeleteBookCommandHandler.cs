using CatalogService.Application.Contracts.Persistence;
using MediatR;

namespace CatalogService.Application.Features.Books.Commands.DeleteBook;

public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, Unit>
{
    private readonly ICatalogRepository _repository;

    public DeleteBookCommandHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(request.Id);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {request.Id} not found");
        }

        await _repository.DeleteAsync(book);
        return Unit.Value;
    }
}
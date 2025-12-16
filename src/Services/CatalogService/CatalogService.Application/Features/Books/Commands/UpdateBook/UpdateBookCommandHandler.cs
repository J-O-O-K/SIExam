using AutoMapper;
using CatalogService.Application.Contracts.Persistence;
using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using MediatR;

namespace CatalogService.Application.Features.Books.Commands.UpdateBook;

public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, BookDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public UpdateBookCommandHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(request.Id);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {request.Id} not found");
        }

        if (!string.IsNullOrEmpty(request.Title))
            book.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Author))
            book.Author = request.Author;

        book.UpdatedAt = DateTime.UtcNow;

        var updatedBook = await _repository.UpdateAsync(book);
        return _mapper.Map<BookDto>(updatedBook);
    }
}
using AutoMapper;
using CatalogService.Application.Contracts.Persistence;
using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using CatalogService.Domain.Entities;
using MediatR;

namespace CatalogService.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, BookDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public CreateBookCommandHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        // Check if book with ISBN already exists
        var existingBook = await _repository.GetByISBNAsync(request.ISBN);
        if (existingBook != null)
        {
            throw new InvalidOperationException($"Book with ISBN {request.ISBN} already exists");
        }

        var book = new Book
        {
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdBook = await _repository.CreateAsync(book);
        return _mapper.Map<BookDto>(createdBook);
    }
}
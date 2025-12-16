using AutoMapper;
using CatalogService.Application.Contracts.Persistence;
using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using MediatR;

namespace CatalogService.Application.Features.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, BookDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public GetBookByIdQueryHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BookDto> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _repository.GetByIdAsync(request.Id);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {request.Id} not found");
        }

        return _mapper.Map<BookDto>(book);
    }
}
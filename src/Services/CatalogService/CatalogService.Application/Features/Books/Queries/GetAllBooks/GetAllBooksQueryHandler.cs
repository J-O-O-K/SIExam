using AutoMapper;
using CatalogService.Application.Contracts.Persistence;
using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using MediatR;

namespace CatalogService.Application.Features.Books.Queries.GetAllBooks;

public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookDto>>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public GetAllBooksQueryHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BookDto>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var books = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<BookDto>>(books);
    }
}
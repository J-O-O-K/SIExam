using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Application.DTOs.BookDTOs;
using CatalogService.Domain.Entities;

namespace CatalogService.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>();
        CreateMap<CreateBookDto, Book>();
        CreateMap<UpdateBookDto, Book>();
    }
}
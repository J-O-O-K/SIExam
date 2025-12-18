using AutoMapper;
using LendingService.Application.DTOs;
using LendingService.Domain.Entities;

namespace LendingService.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Loan, LoanDto>();
    }
}
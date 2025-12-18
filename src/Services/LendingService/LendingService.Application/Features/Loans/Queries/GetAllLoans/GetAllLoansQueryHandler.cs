using AutoMapper;
using LendingService.Application.Contracts.Persistence;
using LendingService.Application.DTOs;
using MediatR;

namespace LendingService.Application.Features.Loans.Queries.GetAllLoans;

public class GetAllLoansQueryHandler : IRequestHandler<GetAllLoansQuery, IEnumerable<LoanDto>>
{
    private readonly ILendingRepository _repository;
    private readonly IMapper _mapper;

    public GetAllLoansQueryHandler(ILendingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoanDto>> Handle(GetAllLoansQuery request, CancellationToken cancellationToken)
    {
        var loans = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<LoanDto>>(loans);
    }
}
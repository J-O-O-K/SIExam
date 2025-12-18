using AutoMapper;
using LendingService.Application.Contracts.Persistence;
using LendingService.Application.DTOs;
using MediatR;

namespace LendingService.Application.Features.Loans.Queries.GetUserLoans;

public class GetUserLoansQueryHandler : IRequestHandler<GetUserLoansQuery, IEnumerable<LoanDto>>
{
    private readonly ILendingRepository _repository;
    private readonly IMapper _mapper;

    public GetUserLoansQueryHandler(ILendingRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoanDto>> Handle(GetUserLoansQuery request, CancellationToken cancellationToken)
    {
        var loans = await _repository.GetByUserIdAsync(request.UserId);
        return _mapper.Map<IEnumerable<LoanDto>>(loans);
    }
}
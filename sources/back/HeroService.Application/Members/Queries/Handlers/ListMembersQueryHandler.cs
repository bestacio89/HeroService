

using Franz.Common.Business.Domain;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using HeroService.Contracts.DTOs;
using HeroService.Domain.Entities;

using HeroService.Contracts.Queries.Members;

namespace HeroService.Application.Members.Queries;

public sealed class ListMembersQueryHandler
    : IQueryHandler<ListMembersQuery, Result<IReadOnlyCollection<MemberDto>>>
{
    private readonly IReadRepository<Member> _memberRepository;
    private readonly IFranzMapper _mapper;

    public ListMembersQueryHandler(IReadRepository<Member> memberRepository, IFranzMapper mapper)
    {
        _memberRepository = memberRepository;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyCollection<MemberDto>>> Handle(ListMembersQuery request, CancellationToken cancellationToken)
    {
        var members = await _memberRepository.GetAll(cancellationToken);
        var mapped = _mapper.Map<IReadOnlyCollection<Member>,IReadOnlyCollection<MemberDto>>((IReadOnlyCollection<Member>)members);

        return mapped.ToResult();
    }
}

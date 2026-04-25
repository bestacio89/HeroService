using HeroService.Application.Members.Queries;
using HeroService.Domain.Entities;

using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;

using HeroService.Contracts.DTOs;
using HeroService.Contracts.Persistence;
using HeroService.Contracts.Queries.Members;

namespace HeroService.Application.Queries.Members.Handlers
{
    public sealed class GetMemberByNameQueryHandler : IQueryHandler<GetMemberByNameQuery, Result<MemberDto>>
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IFranzMapper _mapper;

        public GetMemberByNameQueryHandler(IMemberRepository memberRepository, IFranzMapper mapper)
        {
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        public async Task<Result<MemberDto>> Handle(GetMemberByNameQuery request, CancellationToken cancellationToken)
        {
            var member = await _memberRepository.GetByPredicateAsync(m=> m.Name.ToString().Contains(request.FullName) ,  cancellationToken);

            if (member is null)
                return "Member not found".ToFailure<MemberDto>();

            return _mapper.Map<Member,MemberDto>(member.First()).ToResult();
        }
    }
}

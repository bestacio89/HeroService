// Queries/Members/GetMemberByNameQuery.cs

using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using HeroService.Contracts.DTOs;

namespace HeroService.Contracts.Queries.Members
{
    public sealed record GetMemberByNameQuery(string FullName) : IQuery<Result<MemberDto>>;
}

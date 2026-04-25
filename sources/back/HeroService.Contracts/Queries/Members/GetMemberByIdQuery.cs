// Application/Members/Queries/GetMemberByIdQuery.cs
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using HeroService.Contracts.DTOs;

namespace HeroService.Contracts.Queries.Members;

public sealed record GetMemberByIdQuery(int MemberId)
    : IQuery<Result<MemberDto>>;

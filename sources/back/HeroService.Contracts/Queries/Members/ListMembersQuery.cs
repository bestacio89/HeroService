// Application/Members/Queries/ListMembersQuery.cs
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using HeroService.Contracts.DTOs;

namespace HeroService.Contracts.Queries.Members;

public sealed record ListMembersQuery
    : IQuery<Result<IReadOnlyCollection<MemberDto>>>;

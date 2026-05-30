using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.GameVersions;

namespace HeroService.Contracts.Queries.GameVersions;

public sealed record GetActiveGameVersionQuery()
    : IQuery<GameVersionDto?>;
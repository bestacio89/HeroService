using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Snapshots;

namespace HeroService.Contracts.Queries.Snapshots;

public sealed record GetHeroSnapshotQuery(
    Guid HeroId,
    Guid GameVersionId
) : IQuery<HeroSnapshotDto>;
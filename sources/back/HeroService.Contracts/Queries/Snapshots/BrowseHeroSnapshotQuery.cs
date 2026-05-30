using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Snapshots;

public sealed record BrowseHeroSnapshotsQuery(
    Guid GameVersionId
) : IQuery<IReadOnlyList<HeroSnapshotDto>>;
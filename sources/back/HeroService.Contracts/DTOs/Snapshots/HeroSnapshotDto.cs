using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;

namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record HeroSnapshotDto(
    Guid HeroId,
    string Heroname,
    Guid GameVersionId,
    HeroStatSnapshotDto Stats,
    HeroSkillKitSnapshotDto Skills,
    HeroKitProfileDto KitProfile
);
namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record HeroSkillKitSnapshotDto(
    SkillSnapshotDto Passive,
    SkillSnapshotDto Primary,
    SkillSnapshotDto Secondary,
    SkillSnapshotDto Tertiary,
    SkillSnapshotDto Ultimate
);
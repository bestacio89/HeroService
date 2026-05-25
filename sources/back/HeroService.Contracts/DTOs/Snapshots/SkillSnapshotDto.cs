namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record SkillSnapshotDto(
    Guid SkillId,
    SkillExecutionSnapshotDto Execution,
    SkillEffectSnapshotDto Effects
);
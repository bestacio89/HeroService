namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record SkillSnapshotDto(
    Guid SkillId,
    string SkilleName,
    SkillExecutionSnapshotDto Execution,
    SkillEffectSnapshotDto Effects,
    IReadOnlyList<EffectExecutionSnapshotDto> EffectExecutions
);
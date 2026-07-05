using HeroService.Domain.Heroes.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record EffectExecutionSnapshotDto
(
    EffectType EffectType,

    Guid SourceSkillId,
    Guid CasterId,

    IReadOnlyList<Guid> TargetIds,

    float FinalMagnitude,
    float FinalDuration,
    float FinalRadius,

    int StacksApplied,

    bool IsInstant,
    bool IsPeriodic,
    bool IsChannelled,

    float TickInterval,
    float ChannelDuration,

    TargetType TargetType
);

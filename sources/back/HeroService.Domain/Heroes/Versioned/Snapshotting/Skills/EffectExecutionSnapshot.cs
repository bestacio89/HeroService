using HeroService.Domain.Heroes.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

public sealed record EffectExecutionSnapshot
(
    // =========================================================
    // IDENTITY
    // =========================================================

    EffectType EffectType,

    BuffType? BuffType,
    DebuffType? DebuffType,

    Guid SourceSkillId,
    Guid CasterId,

    IReadOnlyList<Guid> TargetIds,


    // =========================================================
    // RESOLVED NUMERICS
    // =========================================================

    float FinalMagnitude,
    float FinalDuration,
    float FinalRadius,

    int StacksApplied,


    // =========================================================
    // EXECUTION MODE
    // =========================================================

    bool IsInstant,
    bool IsPeriodic,
    bool IsChannelled,

    float TickInterval,
    float ChannelDuration,


    // =========================================================
    // TARGETING
    // =========================================================

    TargetType TargetType
);
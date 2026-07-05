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

    Guid SourceSkillId,
    Guid CasterId,

    IReadOnlyList<Guid> TargetIds,

    // =========================================================
    // RESOLVED NUMERICS (FROM SkillEffect + Stats)
    // =========================================================

    float FinalMagnitude,     // derived from Magnitude + AD/AP/HP ratios
    float FinalDuration,      // derived from Duration + modifiers
    float FinalRadius,        // derived from Radius + buffs

    int StacksApplied,        // resolved via StackType + MaxStacks

    // =========================================================
    // EXECUTION MODE
    // =========================================================

    bool IsInstant,
    bool IsPeriodic,
    bool IsChannelled,

    float TickInterval,       // ONLY if IsPeriodic
    float ChannelDuration,    // ONLY if IsChannelled

    // =========================================================
    // TARGETING RESULT
    // =========================================================

    TargetType TargetType
);
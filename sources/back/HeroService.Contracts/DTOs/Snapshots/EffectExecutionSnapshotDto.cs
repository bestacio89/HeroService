using HeroService.Domain.Heroes.Skills;
using System;
using System.Collections.Generic;

namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record EffectExecutionSnapshotDto
(
    // =========================================================
    // IDENTITY
    // =========================================================

    EffectType EffectType,


    // =========================================================
    // STATE MODIFIER SEMANTICS
    // =========================================================

    BuffType? BuffType,

    DebuffType? DebuffType,


    Guid SourceSkillId,

    Guid CasterId,

    IReadOnlyList<Guid> TargetIds,


    // =========================================================
    // RESOLVED VALUES
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
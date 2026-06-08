namespace HeroService.Contracts.DTOs.Snapshots;

/// <summary>
/// DTO mirror of SkillEffectSnapshot.
/// Must remain a strict 1:1 mapping with SkillEffectSnapshot —
/// any addition to EffectType propagates here via SkillEffectSnapshot.
/// </summary>
public sealed record SkillEffectSnapshotDto(

    // DIRECT COMBAT OUTPUT
    bool HasDamage,
    bool HasDamageOverTime,
    bool HasHeal,
    bool HasHealOverTime,
    bool HasShield,

    // CONTROL SYSTEM
    bool HasCrowdControl,

    // STATE MODIFIERS
    bool HasBuff,
    bool HasDebuff,

    // POSITIONING / MOVEMENT SYSTEM
    bool HasMobility,

    // EXECUTION SYSTEM
    bool HasExecute,

    // UTILITY / STRATEGIC EFFECTS
    bool HasUtility,
    bool HasVision,
    bool HasZoneControl,
    bool HasSummon,
    bool HasTransformation
);
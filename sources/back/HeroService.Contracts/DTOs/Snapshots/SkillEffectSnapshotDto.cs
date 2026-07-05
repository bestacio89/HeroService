namespace HeroService.Contracts.DTOs.Snapshots;

/// <summary>
/// DTO mirror of SkillEffectSnapshot.
/// Must remain a strict 1:1 mapping with SkillEffectSnapshot —
/// any addition to EffectType propagates here via SkillEffectSnapshot.
///
/// HasAnyCrowdControl is derived, not a constructor parameter — mirrors
/// the same convenience property on the domain SkillEffectSnapshot so
/// API consumers (e.g. Unity) don't have to OR all 16 CC flags themselves.
/// </summary>
public sealed record SkillEffectSnapshotDto(

    // DIRECT COMBAT OUTPUT
    bool HasDamage,
    bool HasDamageOverTime,
    bool HasHeal,
    bool HasHealOverTime,
    bool HasShield,

    // CONTROL SYSTEM
    bool HasSlow,
    bool HasRoot,
    bool HasStun,
    bool HasSilence,
    bool HasDisarm,
    bool HasBlind,
    bool HasFear,
    bool HasCharm,
    bool HasTaunt,
    bool HasConfuse,
    bool HasSleep,
    bool HasKnockback,
    bool HasKnockUp,
    bool HasPull,
    bool HasFreeze,
    bool HasPetrify,

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
)
{
  public bool HasAnyCrowdControl =>
      HasSlow || HasRoot || HasStun || HasSilence || HasDisarm || HasBlind ||
      HasFear || HasCharm || HasTaunt || HasConfuse || HasSleep ||
      HasKnockback || HasKnockUp || HasPull ||
      HasFreeze || HasPetrify;
}
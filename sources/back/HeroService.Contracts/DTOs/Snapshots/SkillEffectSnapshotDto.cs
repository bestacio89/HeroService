using HeroService.Domain.Heroes.Skills;
using System.Collections.Generic;

namespace HeroService.Contracts.DTOs.Snapshots;

/// <summary>
/// DTO mirror of SkillEffectSnapshot.
/// </summary>
public sealed record SkillEffectSnapshotDto(

    // =========================================================
    // DIRECT COMBAT OUTPUT
    // =========================================================

    bool HasDamage,
    bool HasDamageOverTime,

    bool HasHeal,
    bool HasHealOverTime,

    bool HasShield,


    // =========================================================
    // CONTROL SYSTEM
    // =========================================================

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


    // =========================================================
    // STATE MODIFIERS
    // =========================================================

    bool HasBuff,
    bool HasDebuff,


    // =========================================================
    // POSITIONING
    // =========================================================

    bool HasMobility,


    // =========================================================
    // EXECUTION
    // =========================================================

    bool HasExecute,


    // =========================================================
    // UTILITY
    // =========================================================

    bool HasUtility,
    bool HasVision,
    bool HasZoneControl,
    bool HasSummon,
    bool HasTransformation,


    // =========================================================
    // STATE MODIFIER DETAILS
    // =========================================================

    IReadOnlySet<BuffType> BuffTypes,

    IReadOnlySet<DebuffType> DebuffTypes
)
{
  public bool HasAnyCrowdControl =>
      HasSlow || HasRoot || HasStun || HasSilence || HasDisarm || HasBlind ||
      HasFear || HasCharm || HasTaunt || HasConfuse || HasSleep ||
      HasKnockback || HasKnockUp || HasPull ||
      HasFreeze || HasPetrify;


  public bool HasBuffType(BuffType type)
      => BuffTypes.Contains(type);


  public bool HasDebuffType(DebuffType type)
      => DebuffTypes.Contains(type);
}
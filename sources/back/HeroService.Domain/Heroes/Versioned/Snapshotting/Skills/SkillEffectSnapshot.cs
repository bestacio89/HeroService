using System;
using System.Collections.Generic;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

/// <summary>
/// A boolean fingerprint of the EffectTypes present in a skill.
///
/// Design Contract:
/// This record mirrors the EffectType enum at a high level.
/// Every value in EffectType maps to exactly one Has* property here.
///
/// Additional semantic classifications:
/// - BuffType provides specialization for EffectType.Buff.
/// - DebuffType provides specialization for EffectType.Debuff.
///
/// These collections do not replace the EffectType mirror.
/// They enrich state modifier analysis without exploding the fingerprint.
///
/// Purpose:
/// - Enables fast, allocation-free kit analysis at snapshot time.
/// - Drives HeroKitProfile computation (matchmaking + item affinity).
/// - Does NOT store magnitude, duration, or targeting information.
///   Those live in EffectExecutionSnapshot.
///
/// Relationships:
/// - Produced by SnapshotResolver.BuildEffects()
/// - Consumed by SnapshotResolver.BuildKitProfile()
/// - Stored inside SkillSnapshot
/// </summary>
public sealed record SkillEffectSnapshot(

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
  // POSITIONING / MOVEMENT SYSTEM
  // =========================================================

  bool HasMobility,


  // =========================================================
  // EXECUTION SYSTEM
  // =========================================================

  bool HasExecute,


  // =========================================================
  // UTILITY / STRATEGIC EFFECTS
  // =========================================================

  bool HasUtility,
  bool HasVision,
  bool HasZoneControl,
  bool HasSummon,
  bool HasTransformation,


  // =========================================================
  // STATE MODIFIER SEMANTICS
  // =========================================================

  IReadOnlySet<BuffType> BuffTypes,

  IReadOnlySet<DebuffType> DebuffTypes

)
{
  /// <summary>
  /// Derived convenience property.
  /// 
  /// Returns true if any crowd control effect exists.
  /// Does not store additional state.
  /// </summary>
  public bool HasAnyCrowdControl =>
      HasSlow || HasRoot || HasStun || HasSilence || HasDisarm || HasBlind ||
      HasFear || HasCharm || HasTaunt || HasConfuse || HasSleep ||
      HasKnockback || HasKnockUp || HasPull ||
      HasFreeze || HasPetrify;


  /// <summary>
  /// Returns true when the skill applies a specific buff category.
  /// </summary>
  public bool HasBuffType(BuffType type) =>
      BuffTypes.Contains(type);


  /// <summary>
  /// Returns true when the skill applies a specific debuff category.
  /// </summary>
  public bool HasDebuffType(DebuffType type) =>
      DebuffTypes.Contains(type);
}
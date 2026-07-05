using System;
using System.Collections.Generic;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

/// <summary>
/// A boolean fingerprint of the EffectTypes present in a skill.
///
/// Design Contract:
/// This record must remain a strict 1:1 mirror of the EffectType enum.
/// Every value in EffectType maps to exactly one Has* property here.
/// Any addition to EffectType requires a corresponding addition here,
/// plus an update to SnapshotResolver.BuildEffects().
///
/// HasAnyCrowdControl below is a *derived* convenience property, not a
/// stored field — it does not violate the 1:1 mirror contract, it just
/// saves callers from OR-ing all 16 CC flags by hand (e.g. HeroKitProfile's
/// CrowdControlSkillCount / IsControlOriented).
///
/// Purpose:
/// - Enables fast, allocation-free kit analysis at snapshot time.
/// - Drives HeroKitProfile computation (matchmaking + item affinity).
/// - Does NOT store magnitude, duration, or target information.
///   Those live in SkillExecutionSnapshot and SkillEffect respectively.
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
  bool HasTransformation
)
{
  /// <summary>
  /// Derived, not stored — true if any of the 16 granular CC flags is set.
  /// Kept for callers (like HeroKitProfile) that only care about CC density,
  /// not which specific CC types are present.
  /// </summary>
  public bool HasAnyCrowdControl =>
      HasSlow || HasRoot || HasStun || HasSilence || HasDisarm || HasBlind ||
      HasFear || HasCharm || HasTaunt || HasConfuse || HasSleep ||
      HasKnockback || HasKnockUp || HasPull ||
      HasFreeze || HasPetrify;
}
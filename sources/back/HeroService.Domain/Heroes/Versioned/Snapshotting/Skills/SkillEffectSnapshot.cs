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

  bool HasCrowdControl,

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
);
using System;

namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Defines the semantic category of a SkillEffect.
/// 
/// Domain Role:
/// EffectType represents the *intent and resolution category* of a skill effect,
/// describing what kind of gameplay outcome it produces during combat simulation.
/// 
/// It does NOT define how the effect is executed, only what it represents.
/// The actual execution logic is handled by the SnapshotResolver.
/// 
/// This enum is a **core semantic contract** for the combat simulation engine.
/// 
/// Matchmaking Relevance:
/// - Directly influences combat role classification and team composition analysis.
/// - Used to determine:
///   • Damage vs sustain ratio in a Hero kit.
///   • Crowd control density in team setups.
///   • Burst vs over-time pressure profiles.
///   • Utility vs raw combat effectiveness.
/// - Enables high-level matchmaking heuristics such as:
///   • "Does this team have enough CC?"
///   • "Is this composition too burst-heavy?"
///   • "Does this team lack sustain?"
/// 
/// Invariants:
/// - Each SkillEffect must map to exactly one EffectType.
/// - EffectType is a closed set and should only evolve through explicit game design decisions.
/// - It must remain stable across versions to preserve snapshot determinism.
/// 
/// Relationships:
/// - Used by SkillEffect as its primary classification.
/// - Consumed by SnapshotResolver to route execution logic.
/// - Influences HeroSkillKit evaluation and matchmaking scoring.
/// - Works in conjunction with TargetType and StackType to fully describe effect behavior.
/// 
/// Versioning / Snapshot Impact:
/// - Highly sensitive to balance and gameplay evolution.
/// - Adding or modifying EffectTypes requires:
///   • SnapshotResolver updates
///   • Rebalancing of affected skills
///   • Potential recalibration of matchmaking heuristics
/// 
/// Developer Notes:
/// - DO NOT encode execution logic inside this enum.
/// - DO NOT infer behavior directly from this enum in domain entities.
/// - This is a *semantic classification layer*, not a behavior engine.
/// - Any new EffectType must be reviewed for impact on:
///   • combat simulation
///   • balance system
///   • matchmaking evaluation
/// 
/// Architectural Insight:
/// - EffectType is the "grammar" of your combat system.
/// - SkillBaseStats defines magnitude.
/// - SkillEffect defines structure.
/// - EffectType defines meaning.
/// - SnapshotResolver defines execution.
/// </summary>
public enum EffectType
{
  // =========================================================
  // DIRECT COMBAT OUTPUT
  // =========================================================
  Damage = 0,
  DamageOverTime = 1,
  Heal = 2,
  HealOverTime = 3,
  Shield = 4,

  // =========================================================
  // CONTROL SYSTEM
  // =========================================================
  Slow = 5,
  Root = 6,
  Stun = 7,
  Silence = 8,
  Disarm = 9,
  Blind = 10,

  Fear = 11,
  Charm = 12,
  Taunt = 13,
  Confuse = 14,
  Sleep = 15,

  Knockback = 16,
  KnockUp = 17,
  Pull = 18,

  Freeze = 19,
  Petrify = 20,

  // =========================================================
  // STATE MODIFIERS
  // =========================================================
  Buff = 21,
  Debuff = 22,

  // =========================================================
  // POSITIONING / MOVEMENT SYSTEM
  // =========================================================
  Mobility = 23,

  // =========================================================
  // EXECUTION SYSTEM
  // =========================================================
  Execute = 24,

  // =========================================================
  // UTILITY / STRATEGIC EFFECTS
  // =========================================================
  Utility = 25,
  Vision = 26,
  ZoneControl = 27,
  Summon = 28,
  Transformation = 29
}
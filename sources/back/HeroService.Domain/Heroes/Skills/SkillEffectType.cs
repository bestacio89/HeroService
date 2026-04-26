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
  /// <summary>Direct instant damage applied at resolution time.</summary>
  Damage,

  /// <summary>Instant healing applied to a target.</summary>
  Heal,

  /// <summary>Temporary or permanent absorption buffer applied to a target.</summary>
  Shield,

  /// <summary>Damage applied over time in intervals.</summary>
  DamageOverTime,

  /// <summary>Control effects such as stun, slow, root, silence, knock-up.</summary>
  CrowdControl,

  /// <summary>Positive modifier applied to a unit (stat increase, empowerment).</summary>
  Buff,

  /// <summary>Negative modifier applied to a unit (stat reduction, weakening).</summary>
  Debuff,

  /// <summary>Healing applied over time in intervals.</summary>
  HealOverTime,

  /// <summary>Execution-style effect that triggers enhanced damage under conditions (e.g., low HP threshold).</summary>
  Execute,

  /// <summary>Applies to all skill effects that modify the mobility of a character.</summary>
  Mobility,




}
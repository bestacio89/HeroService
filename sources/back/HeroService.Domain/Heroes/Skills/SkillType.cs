using System;

namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Defines the high-level archetypal category of a Skill.
///
/// Domain Role:
/// SkillType represents the *design intent category* of a Skill at a macro level.
/// It is used to group abilities by their primary gameplay purpose,
/// such as dealing damage, providing utility, control, or mobility.
///
/// SkillType is a **coarse classification layer** and does NOT define execution logic.
/// The actual behavior of a Skill is determined by its SkillEffects during snapshot resolution.
///
/// Matchmaking Relevance:
/// - Used for high-level analysis of Hero kits and team composition balance.
/// - Supports evaluation of:
///   • Damage vs utility ratio in a Hero kit.
///   • Crowd control availability across teams.
///   • Mobility distribution in a composition.
///   • Burst vs sustain orientation of a Hero.
/// - Enables matchmaking heuristics such as:
///   • “Does this team lack mobility?”
///   • “Is this composition too control-heavy?”
///   • “Is damage profile too concentrated in one role?”
///
/// Invariants:
/// - Each Skill must have exactly one primary SkillType.
/// - SkillType must remain stable across gameplay versions unless explicitly rebalanced.
/// - SkillType does NOT dictate behavior; it only describes intent.
/// - Must remain aligned with underlying SkillEffects to avoid classification drift.
///
/// Relationships:
/// - Assigned to a Skill aggregate.
/// - Works alongside:
///   • SkillEffects (behavioral definition)
///   • SkillBaseStats (numerical definition)
///   • SnapshotResolver (execution engine)
///
/// Versioning / Snapshot Impact:
/// - Medium to high impact depending on reclassification scope.
/// - Changing SkillType can affect:
///   • Hero role evaluation
///   • Matchmaking classification
///   • Team composition heuristics
/// - Requires validation against existing HeroSkillKit compositions when modified.
///
/// Developer Notes:
/// - DO NOT use SkillType as a substitute for logic branching.
/// - DO NOT assume execution behavior from this enum.
/// - SkillType is a *semantic label*, not a runtime rule.
/// - Any gameplay behavior must be derived from SkillEffects and resolved in SnapshotResolver.
///
/// Architectural Insight:
/// - SkillType defines “what the skill is supposed to represent.”
/// - SkillEffects define “what the skill actually does.”
/// - SkillBaseStats define “how strong the skill is.”
/// - SnapshotResolver defines “what actually happens in combat.”
/// </summary>
public enum SkillType
{
  /// <summary>Primary damage-dealing abilities (burst or sustained).</summary>
  Damage,

  /// <summary>Healing abilities applied to self or allies.</summary>
  Heal,

  /// <summary>Protective abilities that absorb or mitigate damage.</summary>
  Shield,

  /// <summary>Movement-based abilities such as dashes, blinks, or repositioning tools.</summary>
  Mobility,

  /// <summary>Control-based abilities such as stun, root, silence, knock-up, slow.</summary>
  Control,

  /// <summary>Positive enhancements applied to self or allies.</summary>
  Buff,

  /// <summary>Negative status effects applied to enemies.</summary>
  Debuff,

  /// <summary>High-impact ultimate abilities with enhanced scaling or special rules.</summary>
  Ultimate
}
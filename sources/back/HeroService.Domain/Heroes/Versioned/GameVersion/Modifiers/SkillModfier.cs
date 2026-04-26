using System;

namespace HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

/// <summary>
/// Represents a versioned balance modification applied to a Skill within a specific GameVersion.
///
/// Domain Role:
/// SkillModifier is a **non-destructive tuning layer** that adjusts SkillBaseStats
/// to reflect balance changes across patches, seasons, or meta updates.
///
/// It does NOT redefine the Skill.
/// It modifies its numerical behavior during snapshot resolution.
///
/// -------------------------
/// DESIGN INTENT
/// -------------------------
/// SkillModifier exists to allow:
/// - Buffs / nerfs per patch
/// - Meta adjustments without rewriting base design
/// - Controlled tuning of skill performance
///
/// It is the **skill-level equivalent of HeroModifier**.
///
/// -------------------------
/// MATCHMAKING RELEVANCE
/// -------------------------
/// SkillModifier directly influences matchmaking simulation by altering:
/// - Damage output potential
/// - Resource efficiency (mana economy)
/// - Ability tempo (cooldowns)
/// - Burst windows and DPS curves
///
/// These changes propagate into:
/// - HeroSnapshot power evaluation
/// - Combat simulation outcomes
/// - Meta composition analysis
///
/// -------------------------
/// INVARIANTS
/// -------------------------
/// - SkillId must reference a valid Skill aggregate
/// - GameVersionId must reference a valid version context
/// - Only ONE modifier per Skill per GameVersion is allowed
/// - All values are optional (null = no change)
/// - Default interpretation is neutral (×1 multiplier or +0 delta depending on axis)
///
/// -------------------------
/// VERSIONING IMPACT
/// -------------------------
/// - SkillModifier is tightly coupled to GameVersion
/// - Any modification affects:
///   • global balance state
///   • hero performance indirectly
///   • matchmaking evaluation stability
///
/// - All changes are snapshot-based and immutable once applied
///
/// -------------------------
/// ARCHITECTURAL ROLE
/// -------------------------
/// SkillBaseStats = design intent
/// SkillEffects = behavior definition
/// SkillModifier = balance tuning layer
/// SkillSnapshot = final deterministic execution state
///
/// -------------------------
/// IMPORTANT RULE
/// -------------------------
/// This class must remain purely declarative.
/// No logic, no computation, no behavior.
/// </summary>
public class SkillModifier : Entity<Guid>
{
  public Guid GameVersionId { get; private set; }
  public Guid SkillId { get; private set; }

  // =========================
  // CORE EXECUTION COST AXIS
  // =========================
  public float? CooldownMultiplier { get; private set; }
  public float? ManaCostMultiplier { get; private set; }

  // =========================
  // OUTPUT AXIS (DAMAGE MODEL)
  // =========================
  public float? DamageMultiplier { get; private set; }
  public float? HealingMultiplier { get; private set; }
  public float? ShieldMultiplier { get; private set; }

  // =========================
  // TEMPO AXIS
  // =========================
  public float? CastTimeMultiplier { get; private set; }
  public float? ChannelDurationMultiplier { get; private set; }

  // =========================
  // UTILITY AXIS
  // =========================
  public float? CrowdControlDurationMultiplier { get; private set; }
  public float? RangeMultiplier { get; private set; }

  // =========================
  // SCALING AXIS
  // =========================
  public float? AttackDamageRatioMultiplier { get; private set; }
  public float? AbilityPowerRatioMultiplier { get; private set; }
  public float? MaxHealthRatioMultiplier { get; private set; }

  private SkillModifier() { }
}
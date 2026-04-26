namespace HeroService.Domain.Heroes.Skills;
/// <summary>
/// Represents the immutable baseline statistical definition of a Skill.
///
/// Domain Role:
/// Defines the raw, unmodified numerical properties of a Skill before any
/// modifiers, scaling effects, item interactions, or Hero-based adjustments
/// are applied.
///
/// This entity is the **fundamental balance unit for ability execution**.
///
/// Matchmaking Relevance:
/// - Indirect but critical influence on matchmaking evaluation via simulation.
/// - Used to:
///   • Estimate burst potential (BaseDamage).
///   • Determine resource consumption efficiency (BaseManaCost).
///   • Evaluate ability tempo and frequency (BaseCooldown).
/// - Combined with HeroBaseStats and SkillEffects to simulate:
///   • Combat rotation strength
///   • Time-to-kill estimations
///   • Utility vs damage balance in team composition analysis
///
/// Invariants:
/// - SkillId must reference a valid Skill aggregate.
/// - All values must be non-negative and validated at creation time.
/// - Each Skill must have exactly one SkillBaseStats definition.
/// - Values represent **pure baseline state only** (no modifiers, no scaling, no buffs).
///
/// Relationships:
/// - Directly associated with a Skill entity (SkillId).
/// - Consumed by:
///   • SnapshotResolver (core deterministic simulation engine)
///   • Combat simulation systems
///   • Matchmaking evaluators (indirect via HeroSkillKit)
/// - Works in conjunction with:
///   • SkillEffects (behavioral layer)
///   • HeroSkillKit (execution context)
///   • HeroBaseStats (global scaling context)
///
/// Versioning / Snapshot Impact:
/// - Highly sensitive to balance changes.
/// - Any modification requires a new version/snapshot generation.
/// - Directly impacts:
///   • Combat outcomes
///   • Matchmaking scoring
///   • Meta stability
/// - Frequently cached and precomputed in snapshot pipelines.
///
/// Developer Notes:
/// - This is a pure simulation input model; do NOT embed behavior here.
/// - Avoid introducing derived stats (crit scaling, dynamic cooldown reduction, etc.).
/// - Changes to this entity are **game balance decisions, not engineering changes**.
/// - Ensure strict validation at creation time to avoid invalid skill configurations.
/// - Keep this model stable and predictable for deterministic simulation.
///
/// Architectural Insight:
/// - SkillBaseStats defines "how strong an ability is at baseline".
/// - SkillEffects define "what the ability does".
/// - Together they form the full deterministic definition of a Skill.
/// - This separation ensures clean snapshot-based simulation without ambiguity.
/// </summary>
public class SkillBaseStats : Entity<Guid>
{
  public Guid SkillId { get; private set; }

  // =========================
  // CORE EXECUTION COSTS
  // =========================
  public float BaseCooldown { get; private set; }
  public float BaseManaCost { get; private set; }

  // =========================
  // OUTPUT POTENTIAL
  // =========================
  public float BaseDamage { get; private set; }
  public float BaseHealing { get; private set; }
  public float BaseShieldValue { get; private set; }

  // =========================
  // TEMPO INTERACTION
  // =========================
  public float BaseCastTime { get; private set; }
  public float BaseChannelDuration { get; private set; }

  // =========================
  // SCALING COEFFICIENTS (CRITICAL ADDITION)
  // =========================
  public float AttackDamageRatio { get; private set; }
  public float AbilityPowerRatio { get; private set; }
  public float MaxHealthRatio { get; private set; }

  // =========================
  // UTILITY POWER AXIS
  // =========================
  public float BaseCrowdControlDuration { get; private set; }
  public float BaseRange { get; private set; }

  private SkillBaseStats() { }

  public SkillBaseStats(
    Guid skillId,
    float baseCooldown,
    float baseManaCost,
    float baseDamage,
    float baseHealing,
    float baseShieldValue,
    float baseCastTime,
    float baseChannelDuration,
    float attackDamageRatio,
    float abilityPowerRatio,
    float maxHealthRatio,
    float baseCrowdControlDuration,
    float baseRange)
  {
    SkillId = skillId;

    BaseCooldown = baseCooldown;
    BaseManaCost = baseManaCost;

    BaseDamage = baseDamage;
    BaseHealing = baseHealing;
    BaseShieldValue = baseShieldValue;

    BaseCastTime = baseCastTime;
    BaseChannelDuration = baseChannelDuration;

    AttackDamageRatio = attackDamageRatio;
    AbilityPowerRatio = abilityPowerRatio;
    MaxHealthRatio = maxHealthRatio;

    BaseCrowdControlDuration = baseCrowdControlDuration;
    BaseRange = baseRange;
  }
}
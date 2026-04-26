/// <summary>
/// Represents the immutable baseline combat statistics of a Hero.
///
/// Domain Role:
/// Defines the raw, unmodified numerical foundation of a Hero's combat capabilities.
/// These values represent the starting point of all gameplay calculations before
/// any modifiers from items, skills, buffs, mythology, or class systems are applied.
///
/// This entity is a **balance anchor** in the entire combat simulation system.
///
/// Matchmaking Relevance:
/// - Serves as a primary quantitative input for matchmaking evaluation.
/// - Used to:
///   • Establish baseline survivability (BaseHealth).
///   • Determine resource sustain potential (BaseMana).
///   • Estimate physical damage output (BaseAttackDamage).
///   • Estimate magical/ability scaling potential (BaseAbilityPower).
/// - Combined with SkillKit and classification systems to derive:
///   • Role effectiveness
///   • Combat archetype strength
///   • Team composition balance
///
/// Invariants:
/// - HeroId must reference a valid Hero aggregate.
/// - All values must be non-negative and validated at creation time.
/// - Each Hero must have exactly one HeroBaseStats instance.
/// - Values represent **pure base state only** (no buffs, no modifiers, no scaling applied).
///
/// Relationships:
/// - Directly associated with a single Hero (HeroId).
/// - Works in conjunction with:
///   • HeroSkillKit (behavioral layer)
///   • HeroClass (role expectations)
///   • HeroAffiliation (identity constraints)
/// - Consumed by:
///   • SnapshotResolver (core simulation engine)
///   • Matchmaking evaluators
///   • Combat simulation systems
///
/// Versioning / Snapshot Impact:
/// - Highly sensitive to balance changes.
/// - Any modification requires a new version/snapshot generation.
/// - Directly affects matchmaking outcomes and combat simulation results.
/// - Frequently cached in snapshot projections for performance.
///
/// Developer Notes:
/// - Treat this entity as **pure input data for simulation**, never derived output.
/// - Do not mix derived stats (crit, armor, penetration, etc.) into this model.
/// - Keep this model stable and minimal to ensure predictable matchmaking behavior.
/// - Validation should occur at creation time (fail fast on invalid stat ranges).
/// - Changes to this structure should be considered a **game balance decision**, not a code change.
///
/// Architectural Insight:
/// - This entity forms the numerical backbone of the Hero system.
/// - Together with HeroSkillKit, it defines:
///   • "What the Hero can do" (skills)
///   • "How strong the Hero is by default" (base stats)
/// - All higher-level systems (items, mythology, class modifiers) layer on top of this.
/// </summary>




namespace HeroService.Domain.Heroes.Core;

public class HeroBaseStats : Entity<Guid>
{
  public Guid HeroId { get; private set; }

  // =========================
  // PRIMARY RESOURCES
  // =========================
  public float BaseHealth { get; private set; }
  public float BaseMana { get; private set; }

  // =========================
  // OFFENSIVE STATS
  // =========================
  public float BaseAttackDamage { get; private set; }
  public float BaseAbilityPower { get; private set; }

  public float BaseAttackSpeed { get; private set; }
  public float BaseCritChance { get; private set; }
  public float BaseCritDamageMultiplier { get; private set; }

  // =========================
  // DEFENSIVE STATS
  // =========================
  public float BaseArmor { get; private set; }
  public float BaseMagicResistance { get; private set; }
  public float BaseDamageReduction { get; private set; }

  public float BaseShieldStrengthMultiplier { get; private set; }

  // =========================
  // TEMPO / MOBILITY
  // =========================
  public float BaseMovementSpeed { get; private set; }
  public float BaseAttackRange { get; private set; }
  public float BaseCastSpeed { get; private set; }

  // =========================
  // COOLDOWN / ECONOMY
  // =========================
  public float BaseCooldownReduction { get; private set; }
  public float BaseResourceRegeneration { get; private set; }

  // =========================
  // SCALING CONTROL (IMPORTANT FOR SNAPSHOT SYSTEM)
  // =========================
  public float HealthScalingPerLevel { get; private set; }
  public float ManaScalingPerLevel { get; private set; }
  public float AttackDamageScalingPerLevel { get; private set; }
  public float AbilityPowerScalingPerLevel { get; private set; }

  private HeroBaseStats() { }

  public HeroBaseStats(
    Guid heroId,

    float baseHealth,
    float baseMana,

    float baseAttackDamage,
    float baseAbilityPower,

    float baseAttackSpeed,
    float baseCritChance,
    float baseCritDamageMultiplier,

    float baseArmor,
    float baseMagicResistance,
    float baseDamageReduction,

    float baseShieldStrengthMultiplier,

    float baseMovementSpeed,
    float baseAttackRange,
    float baseCastSpeed,

    float baseCooldownReduction,
    float baseResourceRegeneration,

    float healthScalingPerLevel,
    float manaScalingPerLevel,
    float attackDamageScalingPerLevel,
    float abilityPowerScalingPerLevel
  )
  {
    HeroId = heroId;

    BaseHealth = baseHealth;
    BaseMana = baseMana;

    BaseAttackDamage = baseAttackDamage;
    BaseAbilityPower = baseAbilityPower;

    BaseAttackSpeed = baseAttackSpeed;
    BaseCritChance = baseCritChance;
    BaseCritDamageMultiplier = baseCritDamageMultiplier;

    BaseArmor = baseArmor;
    BaseMagicResistance = baseMagicResistance;
    BaseDamageReduction = baseDamageReduction;

    BaseShieldStrengthMultiplier = baseShieldStrengthMultiplier;

    BaseMovementSpeed = baseMovementSpeed;
    BaseAttackRange = baseAttackRange;
    BaseCastSpeed = baseCastSpeed;

    BaseCooldownReduction = baseCooldownReduction;
    BaseResourceRegeneration = baseResourceRegeneration;

    HealthScalingPerLevel = healthScalingPerLevel;
    ManaScalingPerLevel = manaScalingPerLevel;
    AttackDamageScalingPerLevel = attackDamageScalingPerLevel;
    AbilityPowerScalingPerLevel = abilityPowerScalingPerLevel;
  }
}
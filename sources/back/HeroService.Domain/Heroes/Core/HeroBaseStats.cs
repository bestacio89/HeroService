namespace HeroService.Domain.Heroes.Core;

public class HeroBaseStats : Entity<Guid>
{
  public Guid HeroId { get; private set; }

  // =========================
  // CORE SURVIVABILITY
  // =========================
  public float BaseHealth { get; private set; }
  public float BaseMana { get; private set; }

  // =========================
  // DAMAGE PROFILE
  // =========================
  public float BaseAttackDamage { get; private set; }
  public float BaseMagicDamage { get; private set; }

  // =========================
  // OFFENSIVE PENETRATION PROFILE
  // =========================
  /// <summary>
  /// Percentage of enemy defenses ignored when dealing damage.
  /// Applies to both armor and magic resistance.
  /// Value range: 0.0 - 1.0
  /// Example: 0.25 = 25% enemy defense ignored.
  /// </summary>
  public float BaseIgnoreEnemyDefense { get; private set; }

  // =========================
  // TEMPO / COMBAT FLOW
  // =========================
  public float BaseAttackSpeed { get; private set; }
  public float BaseCastSpeed { get; private set; }

  // =========================
  // CRITICAL PROFILE
  // =========================
  public float BaseCritChance { get; private set; }
  public float BaseCritDamageMultiplier { get; private set; }

  // =========================
  // DEFENSIVE PROFILE
  // =========================
  public float BaseArmor { get; private set; }
  public float BaseMagicResistance { get; private set; }
  public float BaseDamageReduction { get; private set; }

  public float BaseShieldStrengthMultiplier { get; private set; }

  // =========================
  // POSITIONING PROFILE
  // =========================
  public float BaseMovementSpeed { get; private set; }
  public float BaseAttackRange { get; private set; }

  // =========================
  // RESOURCE PROFILE
  // =========================
  public float BaseCooldownReduction { get; private set; }
  public float BaseResourceRegeneration { get; private set; }

  protected HeroBaseStats(Guid id) : base(id) { }

  public void Define(
      Guid heroId,

      float baseHealth,
      float baseMana,

      float baseAttackDamage,
      float baseMagicDamage,

      float baseIgnoreEnemyDefense,

      float baseAttackSpeed,
      float baseCastSpeed,

      float baseCritChance,
      float baseCritDamageMultiplier,

      float baseArmor,
      float baseMagicResistance,
      float baseDamageReduction,

      float baseShieldStrengthMultiplier,

      float baseMovementSpeed,
      float baseAttackRange,

      float baseCooldownReduction,
      float baseResourceRegeneration,

      string createdBy)
  {
    if (heroId == Guid.Empty)
      throw new ArgumentException("HeroId is required.");

    if (baseIgnoreEnemyDefense is < 0 or > 1)
      throw new ArgumentOutOfRangeException(nameof(baseIgnoreEnemyDefense),
          "Ignore Enemy Defense must be between 0 and 1.");

    HeroId = heroId;

    BaseHealth = baseHealth;
    BaseMana = baseMana;

    BaseAttackDamage = baseAttackDamage;
    BaseMagicDamage = baseMagicDamage;

    BaseIgnoreEnemyDefense = baseIgnoreEnemyDefense;

    BaseAttackSpeed = baseAttackSpeed;
    BaseCastSpeed = baseCastSpeed;

    BaseCritChance = baseCritChance;
    BaseCritDamageMultiplier = baseCritDamageMultiplier;

    BaseArmor = baseArmor;
    BaseMagicResistance = baseMagicResistance;
    BaseDamageReduction = baseDamageReduction;

    BaseShieldStrengthMultiplier = baseShieldStrengthMultiplier;

    BaseMovementSpeed = baseMovementSpeed;
    BaseAttackRange = baseAttackRange;

    BaseCooldownReduction = baseCooldownReduction;
    BaseResourceRegeneration = baseResourceRegeneration;

    MarkCreated(createdBy);
  }
}
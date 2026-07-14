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

    HeroId = heroId;

    BaseHealth = baseHealth;
    BaseMana = baseMana;

    BaseAttackDamage = baseAttackDamage;
    BaseMagicDamage = baseMagicDamage;

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
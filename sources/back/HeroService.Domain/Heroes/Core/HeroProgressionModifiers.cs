namespace HeroService.Domain.Heroes.Progression;

public class HeroProgressionModifiers : Entity<Guid>
{
  public Guid HeroId { get; private set; }

  // =========================
  // SURVIVABILITY SCALING
  // =========================
  public float HealthPerLevel { get; private set; }
  public float ManaPerLevel { get; private set; }

  // =========================
  // DAMAGE SCALING
  // =========================
  public float AttackDamagePerLevel { get; private set; }
  public float AbilityPowerPerLevel { get; private set; }

  // =========================
  // DEFENSIVE SCALING
  // =========================
  public float ArmorPerLevel { get; private set; }
  public float MagicResistancePerLevel { get; private set; }

  // =========================
  // TEMPO SCALING
  // =========================
  public float AttackSpeedPerLevel { get; private set; }
  public float CastSpeedPerLevel { get; private set; }

  // =========================
  // RESOURCE SCALING
  // =========================
  public float ResourceRegenerationPerLevel { get; private set; }

  private HeroProgressionModifiers() { }

  public void Define(
      Guid heroId,

      float healthPerLevel,
      float manaPerLevel,

      float attackDamagePerLevel,
      float abilityPowerPerLevel,

      float armorPerLevel,
      float magicResistancePerLevel,

      float attackSpeedPerLevel,
      float castSpeedPerLevel,

      float resourceRegenerationPerLevel,

      string createdBy)
  {
    if (heroId == Guid.Empty)
      throw new ArgumentException("HeroId is required.");

    HeroId = heroId;

    HealthPerLevel = healthPerLevel;
    ManaPerLevel = manaPerLevel;

    AttackDamagePerLevel = attackDamagePerLevel;
    AbilityPowerPerLevel = abilityPowerPerLevel;

    ArmorPerLevel = armorPerLevel;
    MagicResistancePerLevel = magicResistancePerLevel;

    AttackSpeedPerLevel = attackSpeedPerLevel;
    CastSpeedPerLevel = castSpeedPerLevel;

    ResourceRegenerationPerLevel = resourceRegenerationPerLevel;

    MarkCreated(createdBy);
  }
}
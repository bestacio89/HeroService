namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroBaseStatsCreateRequestDto
{
  // =========================
  // CORE SURVIVABILITY
  // =========================
  public float BaseHealth { get; set; }
  public float BaseMana { get; set; }

  // =========================
  // DAMAGE PROFILE
  // =========================
  public float BaseAttackDamage { get; set; }
  public float BaseMagicDamage { get; set; }

  // =========================
  // TEMPO / COMBAT FLOW
  // =========================
  public float BaseAttackSpeed { get; set; }
  public float BaseCastSpeed { get; set; }

  // =========================
  // CRITICAL PROFILE
  // =========================
  public float BaseCritChance { get; set; }
  public float BaseCritDamageMultiplier { get; set; }

  // =========================
  // DEFENSIVE PROFILE
  // =========================
  public float BaseArmor { get; set; }
  public float BaseMagicResistance { get; set; }
  public float BaseDamageReduction { get; set; }
  public float BaseShieldStrengthMultiplier { get; set; }

  // =========================
  // POSITIONING PROFILE
  // =========================
  public float BaseMovementSpeed { get; set; }
  public float BaseAttackRange { get; set; }

  // =========================
  // RESOURCE PROFILE
  // =========================
  public float BaseCooldownReduction { get; set; }
  public float BaseResourceRegeneration { get; set; }
}
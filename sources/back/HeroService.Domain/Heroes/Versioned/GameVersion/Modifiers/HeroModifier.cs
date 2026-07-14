using System;

namespace HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

/// <summary>
/// Versioned balance layer applied to HeroBaseStats during snapshot resolution.
///
/// Domain Role:
/// HeroStatsModifier adjusts the final combat performance of a Hero
/// without modifying its base identity or progression curve.
///
/// It represents patch-level tuning only.
/// </summary>
public class HeroModifier : Entity<Guid>
{
  public Guid GameVersionId { get; private set; }
  public Guid HeroId { get; private set; }

  // =========================
  // CORE SURVIVABILITY
  // =========================
  public float? HealthMultiplier { get; private set; }
  public float? ManaMultiplier { get; private set; }

  // =========================
  // DAMAGE PROFILE
  // =========================
  public float? AttackDamageMultiplier { get; private set; }
  public float? MagicDamageMultiplier { get; private set; }

  // =========================
  // TEMPO / COMBAT FLOW
  // =========================
  public float? AttackSpeedMultiplier { get; private set; }
  public float? CastSpeedMultiplier { get; private set; }

  // =========================
  // CRITICAL PROFILE
  // =========================
  public float? CritChanceMultiplier { get; private set; }
  public float? CritDamageMultiplier { get; private set; }

  // =========================
  // DEFENSIVE PROFILE
  // =========================
  public float? ArmorMultiplier { get; private set; }
  public float? MagicResistanceMultiplier { get; private set; }
  public float? DamageReductionMultiplier { get; private set; }

  public float? ShieldStrengthMultiplier { get; private set; }

  // =========================
  // POSITIONING PROFILE
  // =========================
  public float? MovementSpeedMultiplier { get; private set; }
  public float? AttackRangeMultiplier { get; private set; }

  // =========================
  // RESOURCE PROFILE
  // =========================
  public float? CooldownReductionMultiplier { get; private set; }
  public float? ResourceRegenerationMultiplier { get; private set; }

  protected HeroModifier(Guid id) : base(id) { }


  // =========================================================
  // DEFINE
  // =========================================================
  public void Define(
      Guid gameVersionId,
      Guid heroId,

      float? healthMultiplier,
      float? manaMultiplier,

      float? attackDamageMultiplier,
      float? magicDamageMultiplier,

      float? attackSpeedMultiplier,
      float? castSpeedMultiplier,

      float? critChanceMultiplier,
      float? critDamageMultiplier,

      float? armorMultiplier,
      float? magicResistanceMultiplier,
      float? damageReductionMultiplier,

      float? shieldStrengthMultiplier,

      float? movementSpeedMultiplier,
      float? attackRangeMultiplier,

      float? cooldownReductionMultiplier,
      float? resourceRegenerationMultiplier,

      string createdBy)
  {
    if (gameVersionId == Guid.Empty)
      throw new ArgumentException("GameVersionId is required.");

    if (heroId == Guid.Empty)
      throw new ArgumentException("HeroId is required.");

    GameVersionId = gameVersionId;
    HeroId = heroId;

    HealthMultiplier = healthMultiplier;
    ManaMultiplier = manaMultiplier;

    AttackDamageMultiplier = attackDamageMultiplier;
    MagicDamageMultiplier = magicDamageMultiplier;

    AttackSpeedMultiplier = attackSpeedMultiplier;
    CastSpeedMultiplier = castSpeedMultiplier;

    CritChanceMultiplier = critChanceMultiplier;
    CritDamageMultiplier = critDamageMultiplier;

    ArmorMultiplier = armorMultiplier;
    MagicResistanceMultiplier = magicResistanceMultiplier;
    DamageReductionMultiplier = damageReductionMultiplier;

    ShieldStrengthMultiplier = shieldStrengthMultiplier;

    MovementSpeedMultiplier = movementSpeedMultiplier;
    AttackRangeMultiplier = attackRangeMultiplier;

    CooldownReductionMultiplier = cooldownReductionMultiplier;
    ResourceRegenerationMultiplier = resourceRegenerationMultiplier;

    MarkCreated(createdBy);
  }
}
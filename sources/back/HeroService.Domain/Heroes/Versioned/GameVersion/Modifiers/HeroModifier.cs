using System;

namespace HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

public class HeroModifier : Entity<Guid>
{
  public Guid GameVersionId { get; private set; }
  public Guid HeroId { get; private set; }

  // =========================
  // CORE SURVIVABILITY AXIS
  // =========================
  public float? HealthMultiplier { get; private set; }
  public float? ManaMultiplier { get; private set; }

  // =========================
  // DAMAGE AXIS (ALL DAMAGE SOURCES)
  // =========================
  public float? AttackDamageMultiplier { get; private set; }
  public float? AbilityPowerMultiplier { get; private set; }
  public float? CritDamageMultiplier { get; private set; }
  public float? CritChanceMultiplier { get; private set; }

  // =========================
  // COMBAT TEMPO AXIS
  // =========================
  public float? AttackSpeedMultiplier { get; private set; }
  public float? CastSpeedMultiplier { get; private set; }
  public float? CooldownReductionMultiplier { get; private set; }

  // =========================
  // DEFENSIVE AXIS
  // =========================
  public float? ArmorMultiplier { get; private set; }
  public float? MagicResistanceMultiplier { get; private set; }
  public float? DamageReductionMultiplier { get; private set; }
  public float? ShieldStrengthMultiplier { get; private set; }

  // =========================
  // MOBILITY / POSITIONING AXIS
  // =========================
  public float? MovementSpeedMultiplier { get; private set; }
  public float? AttackRangeMultiplier { get; private set; }

  // =========================
  // ECONOMY / SUSTAIN AXIS
  // =========================
  public float? ResourceRegenerationMultiplier { get; private set; }

  // =========================
  // SCALING AXIS (VERY IMPORTANT FOR SNAPSHOTS)
  // =========================
  public float? HealthScalingMultiplier { get; private set; }
  public float? ManaScalingMultiplier { get; private set; }
  public float? AttackDamageScalingMultiplier { get; private set; }
  public float? AbilityPowerScalingMultiplier { get; private set; }

  private HeroModifier() { }
}
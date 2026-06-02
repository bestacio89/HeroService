using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroBaseStatsCreateRequestDto
{
  public float BaseHealth { get; init; }
  public float BaseMana { get; init; }

  public float BaseAttackDamage { get; init; }
  public float BaseAbilityPower { get; init; }

  public float BaseAttackSpeed { get; init; }
  public float BaseCritChance { get; init; }
  public float BaseCritDamageMultiplier { get; init; }

  public float BaseArmor { get; init; }
  public float BaseMagicResistance { get; init; }

  public float BaseDamageReduction { get; init; }   // ✅ ADD

  public float BaseShieldStrengthMultiplier { get; init; } // ✅ ADD

  public float BaseMovementSpeed { get; init; }
  public float BaseAttackRange { get; init; }

  public float BaseCastSpeed { get; init; } // optional but consistent

  public float BaseCooldownReduction { get; init; }
  public float BaseResourceRegeneration { get; init; }

  public float HealthScalingPerLevel { get; init; }
  public float ManaScalingPerLevel { get; init; }
  public float AttackDamageScalingPerLevel { get; init; }
  public float AbilityPowerScalingPerLevel { get; init; }
}
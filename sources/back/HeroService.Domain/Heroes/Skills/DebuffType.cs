namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Defines the specific negative modifier applied by a Debuff SkillEffect.
/// 
/// DebuffType represents the semantic impact, not the implementation.
/// </summary>
public enum DebuffType
{
  ArmorReduction = 0,
  MagicResistanceReduction = 1,

  AttackDamageReduction = 2,
  MagicDamageReduction = 3,

  AttackSpeedReduction = 4,
  MovementSpeedReduction = 5,

  AttackRangeReduction = 6,

  HealingReduction = 7,

  DamageAmplification = 8,
  DamageReduction = 9,

  CooldownIncrease = 10,

  ManaReduction = 11,
  ManaRegenerationReduction = 12,

  VisionReduction = 13,

  ShieldReduction = 14,

  TenacityReduction = 15
}
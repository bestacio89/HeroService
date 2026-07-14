namespace HeroService.Domain.Heroes.Skills;

/// <summary>
/// Defines the specific positive modifier applied by a Buff SkillEffect.
/// 
/// BuffType does not define magnitude or duration.
/// Those values remain controlled by SkillEffect.
/// </summary>
public enum BuffType
{
  Armor = 0,
  MagicResistance = 1,

  AttackDamage = 2,
  AbilityPower = 3,

  AttackSpeed = 4,
  MovementSpeed = 5,

  AttackRange = 6,

  CriticalChance = 7,
  CriticalDamage = 8,

  CooldownReduction = 9,

  HealingPower = 10,
  HealingReceived = 11,

  Lifesteal = 12,
  SpellVamp = 13,

  MaxHealth = 14,
  HealthRegeneration = 15,

  MaxMana = 16,
  ManaRegeneration = 17,

  Tenacity = 18,

  VisionRadius = 19,

  DamageReduction = 20
}
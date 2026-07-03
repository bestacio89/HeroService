using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.DTOs.Skills;

public sealed class SkillEffectDto
{
  public EffectType EffectType { get; set; }

  public float Magnitude { get; set; }
  public float Duration { get; set; }
  public float Radius { get; set; }

  public TargetType TargetType { get; set; }

  public StackType StackType { get; set; }

  public int MaxStacks { get; set; }

  public bool IsPeriodic { get; set; }
  public bool IsInstant { get; set; }
  public bool IsChannelled { get; set; }

  public float? AttackDamageRatio { get; set; }
  public float? AbilityPowerRatio { get; set; }
  public float? MaxHealthRatio { get; set; }
}
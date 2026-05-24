namespace HeroService.Contracts.DTOs.Skills;

public sealed class SkillEffectDto
{
  public string EffectType { get; set; } = string.Empty;

  public float Magnitude { get; set; }
  public float Duration { get; set; }
  public float Radius { get; set; }

  public string TargetType { get; set; } = string.Empty;

  public string StackType { get; set; } = string.Empty;

  public int MaxStacks { get; set; }

  public bool IsPeriodic { get; set; }
  public bool IsInstant { get; set; }
  public bool IsChannelled { get; set; }
  public float? AttackDamageRatio { get; set; }
  public float? AbilityPowerRatio { get; set; }
  public float? MaxHealthRatio { get; set; }
}
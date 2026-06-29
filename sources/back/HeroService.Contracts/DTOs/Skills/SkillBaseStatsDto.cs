namespace HeroService.Contracts.DTOs.Skills;

public sealed class SkillBaseStatsDto
{
  public float? Cooldown { get; set; }
  public float? ManaCost { get; set; }

  public float? Damage { get; set; }
  public float? Healing { get; set; }
  public float? ShieldValue { get; set; }

  public float? CastTime { get; set; }
  public float? ChannelDuration { get; set; }

  public float? Range { get; set; }
  public float? CrowdControlDuration { get; set; }
}
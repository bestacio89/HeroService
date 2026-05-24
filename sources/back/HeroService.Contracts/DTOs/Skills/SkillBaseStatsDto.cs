using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Skills;


public sealed class SkillBaseStatsDto
{
  public float Cooldown { get; set; }
  public float ManaCost { get; set; }

  public float Damage { get; set; }
  public float Healing { get; set; }
  public float ShieldValue { get; set; }

  public float CastTime { get; set; }
  public float ChannelDuration { get; set; }

  public float Range { get; set; }

  public float AttackDamageRatio { get; set; }
  public float AbilityPowerRatio { get; set; }
  public float MaxHealthRatio { get; set; }

  public float CrowdControlDuration { get; set; }
}
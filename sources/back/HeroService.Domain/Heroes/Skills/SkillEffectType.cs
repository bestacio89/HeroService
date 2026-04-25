using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Skills;

public enum EffectType
{
  Damage,
  Heal,
  Shield,
  DamageOverTime,
  CrowdControl,
  Buff,
  Debuff,
  HealOverTime,
  Execute
}
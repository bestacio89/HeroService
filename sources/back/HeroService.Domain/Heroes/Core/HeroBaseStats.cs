using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Core;

public class HeroBaseStats : Entity<Guid>
{
  public Guid HeroId { get; private set; }

  public float BaseHealth { get; private set; }
  public float BaseMana { get; private set; }
  public float BaseAttackDamage { get; private set; }
  public float BaseAbilityPower { get; private set; }

  public HeroBaseStats(Guid heroId, float hp, float mana, float ad, float ap)
  {
    HeroId = heroId;
    BaseHealth = hp;
    BaseMana = mana;
    BaseAttackDamage = ad;
    BaseAbilityPower = ap;
  }
}

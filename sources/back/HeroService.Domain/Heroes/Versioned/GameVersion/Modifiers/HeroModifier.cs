using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

public class HeroModifier : Entity<Guid>
{
  public Guid GameVersionId { get; private set; }
  public Guid HeroId { get; private set; }

  public float? HealthMultiplier { get; private set; }
  public float? DamageMultiplier { get; private set; }

  public float? ManaMultiplier { get; private set; }
}
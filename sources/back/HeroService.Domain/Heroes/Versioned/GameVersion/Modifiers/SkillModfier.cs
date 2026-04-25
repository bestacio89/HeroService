using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

public class SkillModifier : Entity<Guid>
{
  public Guid GameVersionId { get; private set; }
  public Guid SkillId { get; private set; }

  public float? CooldownDelta { get; private set; }
  public float? ManaCostDelta { get; private set; }
  public float? DamageMultiplier { get; private set; }
}
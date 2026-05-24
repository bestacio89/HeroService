using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;

public sealed class HeroSkillKitSnapshot
{
  public SkillSnapshot Passive { get; }
  public SkillSnapshot Primary { get; }
  public SkillSnapshot Secondary { get; }
  public SkillSnapshot Tertiary { get; }
  public SkillSnapshot Ultimate { get; }

  public IReadOnlyList<SkillSnapshot> AllSkills { get; }

  public HeroSkillKitSnapshot(
      SkillSnapshot passive,
      SkillSnapshot primary,
      SkillSnapshot secondary,
      SkillSnapshot tertiary,
      SkillSnapshot ultimate)
  {
    Passive = passive;
    Primary = primary;
    Secondary = secondary;
    Tertiary = tertiary;
    Ultimate = ultimate;

    AllSkills = new[]
    {
            passive, primary, secondary, tertiary, ultimate
        };
  }
}

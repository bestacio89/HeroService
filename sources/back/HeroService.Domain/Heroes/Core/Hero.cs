using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Core.Skills;

namespace HeroService.Domain.Heroes.Core;

public class Hero : Entity<Guid>
{
  public string Name { get; private set; }

  public Guid HeroClassId { get; private set; }
  public HeroClass HeroClass { get; private set; } = null!;

  public Guid HeroAffiliationId { get; private set; }   // ✔ FIX

  public HeroSkillKit SkillKit { get; private set; }

  private Hero() { } // EF

  public Hero(
    string name,
    Guid heroClassId,
    Guid heroAffiliationId,
    HeroSkillKit skillKit,
    string createdBy)
  {
    Name = name;
    HeroClassId = heroClassId;
    HeroAffiliationId = heroAffiliationId;
    SkillKit = skillKit;

    MarkCreated(createdBy);
  }

  // optional controlled mutation
  public void SetAffiliation(Guid affiliationId)
  {
    HeroAffiliationId = affiliationId;
  }
}
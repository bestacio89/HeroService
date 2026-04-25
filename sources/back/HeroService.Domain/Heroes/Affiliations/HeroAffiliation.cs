using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Domain.Heroes.Affiliations;

public class HeroAffiliation : Entity<Guid>
{
  public OriginType OriginType { get; private set; }

  public Guid MythologyTypeId { get; private set; }

  private HeroAffiliation() { }

  public HeroAffiliation(OriginType originType, Guid mythologyTypeId)
  {
    OriginType = originType;
    MythologyTypeId = mythologyTypeId;

    MarkCreated("system");
  }
}
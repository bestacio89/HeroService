using HeroService.Domain.Heroes.Affiliations.Classifications;

public sealed class HeroAffiliation
{
  // FK scalars — used for writes, stored as owned type columns
  public Guid ArchetypeId { get; private set; }
  public Guid MythologyId { get; private set; }
  public Guid OriginCultureId { get; private set; }

  // Navigation properties — hydrated by EF on reads via ThenInclude
  public OriginArchetype Archetype { get; private set; } = null!;
  public MythologyType Mythology { get; private set; } = null!;
  public OriginCulture OriginCulture { get; private set; } = null!;

  private HeroAffiliation() { }

  public HeroAffiliation(
      Guid archetypeId,
      Guid mythologyId,
      Guid originCultureId)
  {
    if (archetypeId == Guid.Empty)
      throw new ArgumentException("ArchetypeId is required.");
    if (mythologyId == Guid.Empty)
      throw new ArgumentException("MythologyId is required.");
    if (originCultureId == Guid.Empty)
      throw new ArgumentException("OriginCultureId is required.");

    ArchetypeId = archetypeId;
    MythologyId = mythologyId;
    OriginCultureId = originCultureId;
  }
}
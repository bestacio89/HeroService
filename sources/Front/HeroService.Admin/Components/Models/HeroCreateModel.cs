public sealed class HeroCreateModel
{
  public string Name { get; set; } = string.Empty;

  public Guid HeroClassId { get; set; }

  public Guid MythologyId { get; set; }

  public Guid OriginCultureId { get; set; }

  public Guid OriginArchetypeId { get; set; }
}
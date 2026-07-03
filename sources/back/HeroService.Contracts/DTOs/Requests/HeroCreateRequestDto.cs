namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroCreateRequestDto
{
  public string Name { get; set; } = string.Empty;

  public Guid HeroClassId { get; set; }
  public Guid MythologyTypeId { get; set; }
  public Guid OriginCultureId { get; set; }
  public Guid OriginArchetypeId { get; set; }

  public HeroBaseStatsCreateRequestDto BaseStats { get; set; } = new();
  public HeroSkillKitCreateRequestDto SkillKit { get; set; } = new();
  public HeroLoreCreateRequestDto Lore { get; set; } = new();
}
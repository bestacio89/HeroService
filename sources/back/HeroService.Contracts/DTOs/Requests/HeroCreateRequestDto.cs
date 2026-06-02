namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroCreateRequestDto
{
  public string Name { get; init; } = string.Empty;

  public string HeroClass { get; init; } = string.Empty;

  public string Mythology { get; init; } = string.Empty;

  public string Archetype { get; init; } = string.Empty;

  public string Culture { get; init; } = string.Empty;

  public HeroBaseStatsCreateRequestDto BaseStats { get; init; } = null!;

  public HeroSkillKitCreateRequestDto SkillKit { get; init; } = null!;

  public HeroLoreCreateRequestDto Lore { get; init; } = null!;
}
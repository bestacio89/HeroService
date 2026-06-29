namespace HeroService.Contracts.DTOs.Skills;

public sealed class SkillDto
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public string SkillType { get; set; } = string.Empty;

  public List<SkillEffectDto> Effects { get; set; } = new();

  public SkillBaseStatsDto BaseStats { get; set; } = default!;
}
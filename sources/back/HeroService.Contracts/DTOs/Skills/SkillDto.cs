using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.DTOs.Skills;

public sealed class SkillDto
{
  public Guid Id { get; set; }

  public string Name { get; set; } = string.Empty;

  public SkillType SkillType { get; set; }

  public List<SkillEffectDto> Effects { get; set; } = new();

  public SkillBaseStatsDto BaseStats { get; set; } = default!;
}
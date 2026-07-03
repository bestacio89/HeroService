using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Admin.Components.Models;

public sealed class CreateSkillModel
{
  public string Name { get; set; } = string.Empty;
  public SkillType SkillType { get; set; }

  public SkillLoreDto Lore { get; set; } = new();
  public SkillBaseStatsDto BaseStats { get; set; } = new();

  public List<SkillEffectDto> Effects { get; set; } = new();
}
using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.Commands.Skills;

public sealed class CreateSkillCommand : ICommand<Guid>
{
  public string Name { get; set; } = string.Empty;


  public SkillType SkillType { get; set; }


  public SkillLoreDto Lore { get; set; } = new();


  public SkillBaseStatsDto BaseStats { get; set; } = new();


  public List<SkillEffectDto> Effects { get; set; } = [];
  public CreateSkillCommand(
    string name,
    SkillType skillType,
    SkillLoreDto lore,
    SkillBaseStatsDto baseStats,
    List<SkillEffectDto> effects)
  {
    Name = name;
    SkillType = skillType;
    Lore = lore;
    BaseStats = baseStats;
    Effects = effects;
  }
}
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
}
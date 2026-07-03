using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;

public sealed record CreateSkillCommand(
    string Name,
    SkillType skillType,
    SkillLoreDto Lore,
    SkillBaseStatsDto BaseStats,
    List<SkillEffectDto> Effects
) : ICommand<Guid>;
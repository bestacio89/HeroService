using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Skills;

public sealed record CreateSkillCommand(
    string Name,
    string SkillType,
    SkillLoreDto Lore,
    SkillBaseStatsDto BaseStats,
    List<SkillEffectDto> Effects
) : ICommand<Guid>;
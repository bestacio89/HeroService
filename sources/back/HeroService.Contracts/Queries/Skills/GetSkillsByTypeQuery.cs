using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;

public sealed record GetSkillsByTypeQuery(
    SkillType SkillType
) : IQuery<IReadOnlyCollection<SkillDto >>;
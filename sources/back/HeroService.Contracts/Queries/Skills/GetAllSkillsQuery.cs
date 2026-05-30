using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Skills;

namespace HeroService.Contracts.Queries.Skills;

public sealed record GetAllSkillsQuery()
    : IQuery<IReadOnlyCollection<SkillDto>>;
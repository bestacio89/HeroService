using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Skills;

namespace HeroService.Contracts.Queries.Skills;

public sealed record GetSkillBaseStatsQuery(
    Guid SkillId
) : IQuery<SkillBaseStatsDto>;
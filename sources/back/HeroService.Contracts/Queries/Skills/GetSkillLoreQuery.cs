using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Skills;

namespace HeroService.Contracts.Queries.Skills;

public sealed record GetSkillLoreQuery(
    Guid SkillId
) : IQuery<SkillLoreDto>;
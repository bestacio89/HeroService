using HeroService.Contracts.DTOs.Skills;
using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Queries.Skills;

public sealed record GetSkillScalingBySkillQuery(Guid SkillId) : IQuery<SkillScalingModifierDto?>;
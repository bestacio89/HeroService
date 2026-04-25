using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.DTOs.Skills;

public sealed record SkillEffectResponse(
    Guid Id,
    EffectType EffectType
);
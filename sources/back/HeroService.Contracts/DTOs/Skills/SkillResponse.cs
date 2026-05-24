using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.DTOs.Skills;

public sealed record SkillResponse(
    Guid Id,
    Guid HeroId,
    string Name,
    SkillType SkillType,
    IReadOnlyCollection<SkillEffectDto> Effects
);
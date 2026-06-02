using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.DTOs.Skills;

public sealed record SkillResponseDto(
    Guid Id,
    Guid HeroId,
    string Name,
    SkillType SkillType,
    IReadOnlyCollection<SkillEffectDto> Effects
);
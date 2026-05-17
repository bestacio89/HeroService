using HeroService.Contracts.DTOs.Heroes;

public sealed record HeroDto(
    Guid Id,
    string Name,
    Guid HeroClassId,
    HeroClassDto Class,
    HeroAffiliationDto Affiliation,
    HeroBaseStatsDto BaseStats,
    HeroSkillKitDto SkillKit
);
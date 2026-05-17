using System;

namespace HeroService.Contracts.DTOs.Heroes;

public sealed record HeroAffiliationDto(
    OriginArchetypeDto Archetype,
    MythologyTypeDto Mythology,
    OriginCultureDto Culture
);
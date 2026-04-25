using HeroService.Domain.Heroes.Affiliations.Classifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Heroes;

public record HeroDto(
    Guid Id,
    string Name,
    Guid HeroClassId,
    OriginType OriginType,
    MythologyType MythologyType
);
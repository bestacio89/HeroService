using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Heroes;

public sealed record HeroLoreDto(
    string Title,
    string Description,
    string BackgroundStory
);
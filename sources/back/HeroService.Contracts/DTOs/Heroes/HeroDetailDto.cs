using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Heroes;

public sealed record HeroDetailsDto
{
  public Guid Id { get; init; }

  public string Name { get; init; } = string.Empty;

  public HeroLoreDto Lore { get; init; } = default!;

  public HeroClassDto Class { get; init; } = default!;

  public HeroAffiliationDto Affiliation { get; init; } = default!;

  public HeroBaseStatsDto BaseStats { get; init; } = default!;
}

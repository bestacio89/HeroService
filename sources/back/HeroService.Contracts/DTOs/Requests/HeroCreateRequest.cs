using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Requests;


public sealed class HeroCreateRequest
{
  public string Name { get; init; } = string.Empty;

  public string HeroClass { get; init; } = string.Empty;

  public string Mythology { get; init; } = string.Empty;

  public string Archetype { get; init; } = string.Empty;

  public string Culture { get; init; } = string.Empty;

  public HeroBaseStatsCreateRequest BaseStats { get; init; } = null!;

  public HeroSkillKitCreateRequest SkillKit { get; init; } = null!;
}
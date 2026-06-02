using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroSkillKitCreateRequestDto
{
  public string PassiveSkill { get; init; } = string.Empty;

  public string PrimarySkill { get; init; } = string.Empty;

  public string SecondarySkill { get; init; } = string.Empty;

  public string TertiarySkill { get; init; } = string.Empty;

  public string UltimateSkill { get; init; } = string.Empty;
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Requests;

public sealed class HeroSkillKitCreateRequestDto
{
  public Guid PassiveSkillId { get; set; }

  public Guid PrimarySkillId { get; set; }

  public Guid SecondarySkillId { get; set; }

  public Guid TertiarySkillId { get; set; }

  public Guid UltimateSkillId { get; set; }
}
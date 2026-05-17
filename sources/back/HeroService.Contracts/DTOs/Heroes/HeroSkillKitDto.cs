using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Heroes;

public sealed record HeroSkillKitDto(
    Guid PassiveSkillId,
    Guid PrimarySkillId,
    Guid SecondarySkillId,
    Guid TertiarySkillId,
    Guid UltimateSkillId
);
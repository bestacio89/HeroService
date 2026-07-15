using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Modifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Queries.Modifiers;

public sealed record GetSkillModifiersByGameVersionQuery(
   
) : IQuery<IReadOnlyList<SkillModifierDto>>;
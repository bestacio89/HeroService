using Franz.Common.Mediator.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Commands.Skills;

public sealed record UpdateSkillLoreCommand(
    Guid SkillLoreId,
    string Name,
    string Description,
    string VisualExplanation,
    string UpdatedBy
) : ICommand;
using Franz.Common.Mediator.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Commands.Skills;

public sealed record UpdateSkillLoreCommand(
    Guid SkillId,
    string Name,
    string Description,
    string VisualExplanation,
    string UpdatedBy
) : ICommand;
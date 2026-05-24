using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Skills;

namespace HeroService.Contracts.Commands.Skills;

public sealed record CreateSkillCommand(
    SkillDto Request
) : ICommand<Guid>;
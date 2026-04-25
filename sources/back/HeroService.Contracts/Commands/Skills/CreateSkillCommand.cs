using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.Commands.Skills;

public sealed record CreateSkillCommand(
    Guid HeroId,
    string Name,
    SkillType SkillType
) : ICommand<Result<Guid>>;
using Franz.Common.Mediator.Messages;
using Franz.Common.Mediator.Results;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.Commands.Skills;

public sealed record CreateSkillEffectCommand(
    Guid SkillId,
    EffectType EffectType
) : ICommand<Result<Guid>>;
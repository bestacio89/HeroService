
using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Heroes;

public sealed record CreateHeroClassCommand(
    string Name
) : ICommand<Guid>;
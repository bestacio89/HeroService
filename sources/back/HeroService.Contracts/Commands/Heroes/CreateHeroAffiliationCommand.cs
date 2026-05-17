using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Heroes;

public sealed record UpdateHeroAffiliationCommand(
    Guid HeroId,
    string Archetype,
    string Mythology,
    string Culture
) : ICommand;
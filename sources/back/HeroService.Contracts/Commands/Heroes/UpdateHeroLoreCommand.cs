using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Heroes.Lore;

public sealed record UpdateHeroLoreCommand(
    string HeroName,
    string Title,
    string Description,
    string BackgroundStory
) : ICommand;
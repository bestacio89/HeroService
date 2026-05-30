using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.GameVersions;

public sealed record DeactivateGameVersionCommand(
    Guid GameVersionId,
    string UpdatedBy
) : ICommand;
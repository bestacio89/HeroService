using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.GameVersions;

public sealed record ActivateGameVersionCommand(
    Guid GameVersionId,
    string UpdatedBy
) : ICommand;
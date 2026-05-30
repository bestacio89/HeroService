using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.GameVersions;

public sealed record RetireGameVersionCommand(
    Guid GameVersionId,
    string UpdatedBy
) : ICommand;
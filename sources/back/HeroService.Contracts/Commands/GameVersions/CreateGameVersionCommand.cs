using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.GameVersions;

public sealed record CreateGameVersionCommand(
    string VersionNumber,
    string VersionName,
    string CreatedBy
) : ICommand<Guid>;
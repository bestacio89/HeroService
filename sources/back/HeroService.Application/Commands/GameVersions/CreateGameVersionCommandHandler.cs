using Franz.Common.Mediator.Handlers;
using HeroService.Application.Services.GameVersions;
using HeroService.Contracts.Commands.GameVersions;

namespace HeroService.Application.Commands.GameVersions;

public sealed class CreateGameVersionCommandHandler
    : ICommandHandler<CreateGameVersionCommand, Guid>
{
  private readonly GameVersionService _service;

  public CreateGameVersionCommandHandler(GameVersionService service)
  {
    _service = service;
  }

  public async Task<Guid> Handle(
      CreateGameVersionCommand request,
      CancellationToken cancellationToken)
  {
    return await _service.CreateAndActivateAsync(
        request.VersionName,
        request.CreatedBy,
        cancellationToken);
  }
}
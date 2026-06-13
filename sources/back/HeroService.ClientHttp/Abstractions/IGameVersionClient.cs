using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Contracts.Commands.GameVersions;

namespace HeroService.Client.Http.Abstractions;

public interface IGameVersionClient
{
  Task<Guid> CreateAsync(
      CreateGameVersionCommand command,
      CancellationToken cancellationToken = default);

  Task<GameVersionDto?> GetActiveAsync(
      CancellationToken cancellationToken = default);

  Task<GameVersionDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);
}
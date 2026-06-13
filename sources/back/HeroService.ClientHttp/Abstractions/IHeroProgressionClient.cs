using HeroService.Contracts.Commands.Heroes;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Client.Http.Abstractions;

public interface IHeroProgressionClient
{
  Task<HeroProgressionModifiersDto?> GetByHeroAsync(
      Guid heroId,
      CancellationToken cancellationToken = default);

  Task<Guid> DefineAsync(
      DefineHeroProgressionCommand command,
      CancellationToken cancellationToken = default);
}
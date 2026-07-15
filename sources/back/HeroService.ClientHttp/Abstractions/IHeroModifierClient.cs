using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.DTOs.Modifiers;

namespace HeroService.Client.Http.Abstractions;

public interface IHeroModifierClient
{
  Task<Guid> CreateAsync(
      CreateHeroModifierCommand command,
      CancellationToken cancellationToken = default);


  Task UpdateAsync(
      Guid id,
      UpdateHeroModifierCommand command,
      CancellationToken cancellationToken = default);


  Task<HeroModifierDto?> GetByHeroAsync(
      Guid heroId,
      CancellationToken cancellationToken = default);


  Task<IReadOnlyList<HeroModifierDto>> GetActiveAsync(
      CancellationToken cancellationToken = default);
}
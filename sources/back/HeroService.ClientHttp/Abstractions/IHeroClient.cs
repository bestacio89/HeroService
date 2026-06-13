using HeroService.Contracts.DTOs.Requests;

namespace HeroService.Client.Http.Abstractions;

public interface IHeroClient
{
  Task<HeroDto?> GetByIdAsync(
      Guid heroId,
      CancellationToken cancellationToken = default);

  Task<HeroDto?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);

  Task<Guid> CreateAsync(
      HeroCreateRequestDto request,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyCollection<HeroDto>> BrowseAsync(
      Guid? heroClassId = null,
      Guid? mythologyTypeId = null,
      Guid? cultureId = null,
      Guid? archetypeId = null,
      CancellationToken cancellationToken = default);
}
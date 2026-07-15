using HeroService.Contracts.DTOs.Snapshots;

namespace HeroService.Client.Http.Abstractions;

public interface IHeroSnapshotClient
{
  Task<HeroSnapshotDto?> GetAsync(
      Guid heroId,
    
      CancellationToken cancellationToken = default);

  Task<IReadOnlyList<HeroSnapshotDto>> BrowseAsync(
            CancellationToken cancellationToken = default);
}
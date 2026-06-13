using HeroService.Contracts.DTOs.Snapshots;

namespace HeroService.Client.Http.Abstractions;

public interface IHeroSnapshotClient
{
  Task<HeroSnapshotDto?> GetAsync(
      Guid heroId,
      Guid gameVersionId,
      CancellationToken cancellationToken = default);

  Task<IReadOnlyList<HeroSnapshotDto>> BrowseAsync(
      Guid gameVersionId,
      CancellationToken cancellationToken = default);
}
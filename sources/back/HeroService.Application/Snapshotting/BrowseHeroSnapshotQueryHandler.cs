using Franz.Common.Caching.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Application.Heroes.Versioned.Snapshotting;
using HeroService.Contracts.DTOs.Snapshots;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Queries.Snapshots;
using HeroService.Domain.Heroes.Versioned.GameVersion;

namespace HeroService.Application.Heroes.Versioned.Snapshotting.Queries;

public sealed class BrowseHeroSnapshotsQueryHandler
    : IQueryHandler<BrowseHeroSnapshotsQuery, IReadOnlyList<HeroSnapshotDto>>
{
  private readonly IHeroRepository _heroRepository;
  private readonly IHeroModifierRepository _heroModifierRepository;
  private readonly IGameVersionRepository _gameVersionRepository;
  private readonly HeroSnapshotResolutionService _resolutionService;
  private readonly ICacheProvider _cache;

  public BrowseHeroSnapshotsQueryHandler(
      IHeroRepository heroRepository,
      IHeroModifierRepository heroModifierRepository,
      IGameVersionRepository gameVersionRepository,
      HeroSnapshotResolutionService resolutionService,
      ICacheProvider cache)
  {
    _heroRepository = heroRepository;
    _heroModifierRepository = heroModifierRepository;
    _gameVersionRepository = gameVersionRepository;
    _resolutionService = resolutionService;
    _cache = cache;
  }

  public async Task<IReadOnlyList<HeroSnapshotDto>> Handle(
      BrowseHeroSnapshotsQuery request,
      CancellationToken cancellationToken)
  {
    var activeVersion = await _gameVersionRepository.GetActiveAsync(
        cancellationToken)
        ?? throw new InvalidOperationException("No active game version exists.");

    var key = HeroSnapshotCaching.BrowseVersionKey(activeVersion.Id);
    var result = await _cache.GetOrSetAsync(
        key,
        ct => ResolveAsync(activeVersion, ct),
        options: null,
        cancellationToken);

    return result.Value;
  }

  private async Task<IReadOnlyList<HeroSnapshotDto>> ResolveAsync(
      GameVersion activeVersion,
      CancellationToken cancellationToken)
  {
    var heroes = await _heroRepository.GetAllForSnapshotAsync(
        cancellationToken);

    if (heroes.Count == 0)
      return Array.Empty<HeroSnapshotDto>();

    var heroModifiers = await _heroModifierRepository.GetByGameVersionIdAsync(
        activeVersion.Id,
        cancellationToken);

    return await _resolutionService.ResolveAsync(
        heroes,
        activeVersion,
        heroModifiers,
        cancellationToken);
  }
}

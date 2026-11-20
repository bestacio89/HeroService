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

public sealed class GetHeroSnapshotQueryHandler
    : IQueryHandler<GetHeroSnapshotQuery, HeroSnapshotDto>
{
  private readonly IHeroRepository _heroRepository;
  private readonly IHeroModifierRepository _heroModifierRepository;
  private readonly IGameVersionRepository _gameVersionRepository;
  private readonly HeroSnapshotResolutionService _resolutionService;
  private readonly ICacheProvider _cache;

  public GetHeroSnapshotQueryHandler(
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

  public async Task<HeroSnapshotDto> Handle(
      GetHeroSnapshotQuery request,
      CancellationToken cancellationToken)
  {
    var activeVersion = await _gameVersionRepository.GetActiveAsync(
        cancellationToken)
        ?? throw new InvalidOperationException("No active game version exists.");

    var key = HeroSnapshotCaching.SingleVersionKey(
        activeVersion.Id,
        request.HeroId);

    var result = await _cache.GetOrSetAsync(
        key,
        ct => ResolveAsync(request, activeVersion, ct),
        options: null,
        cancellationToken);

    return result.Value;
  }

  private async Task<HeroSnapshotDto> ResolveAsync(
      GetHeroSnapshotQuery request,
      GameVersion activeVersion,
      CancellationToken cancellationToken)
  {
    var hero = await _heroRepository.GetForSnapshotAsync(
        request.HeroId,
        cancellationToken)
        ?? throw new InvalidOperationException(
            $"Hero '{request.HeroId}' not found.");

    var heroModifier = await _heroModifierRepository.GetByHeroAndVersionAsync(
        hero.Id,
        activeVersion.Id,
        cancellationToken);

    if (heroModifier is null)
    {
      throw new InvalidOperationException(
          $"Hero '{hero.Name}' has no modifier for active version '{activeVersion.Id}'.");
    }

    var snapshots = await _resolutionService.ResolveAsync(
        [hero],
        activeVersion,
        [heroModifier],
        cancellationToken);

    return snapshots[0];
  }
}

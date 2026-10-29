using Franz.Common.Caching.Abstractions;
using Franz.Common.Mediator.Dispatchers;
using HeroService.Application.Heroes.Versioned.Snapshotting;
using HeroService.Contracts.Queries.Heroes;
using HeroService.Contracts.Queries.Heroes.Classes;
using HeroService.Contracts.Queries.Modifiers;
using HeroService.Contracts.Queries.Skills;
using HeroService.Contracts.Queries.Snapshots;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HeroService.API.Caching;

/// <summary>
/// Warms the primary skill/hero catalogues and snapshot projections before
/// the API begins accepting requests. Query caching handles subsequent reads.
/// </summary>
public static class HeroServiceCacheWarmup
{
  public static async Task WarmAsync(
      IServiceProvider services,
      CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(services);

    await using var scope = services.CreateAsyncScope();
    var scopedServices = scope.ServiceProvider;
    var dispatcher = scopedServices.GetRequiredService<IDispatcher>();
    var cache = scopedServices.GetRequiredService<ICacheProvider>();
    var logger = scopedServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("HeroService.CacheWarmup");

    // Cache the complete skill catalogue, including mapped effects and stats.
    var skills = await dispatcher.SendAsync(
        new GetAllSkillsQuery(),
        cancellationToken);

    // Cache the hero catalogue and its referenced class information.
    var heroes = await dispatcher.SendAsync(
        new BrowseHeroesQuery(null, null, null, null),
        cancellationToken);

    await dispatcher.SendAsync(
        new GetAllHeroClassesQuery(),
        cancellationToken);

    // The browse handler computes the full snapshot projection in one pass.
    // Reuse those results to seed the per-hero keys without recomputing each
    // snapshot or opening hundreds of redundant database queries.
    var snapshots = await dispatcher.SendAsync(
        new BrowseHeroSnapshotsQuery(),
        cancellationToken);

    foreach (var snapshot in snapshots)
    {
      cancellationToken.ThrowIfCancellationRequested();

      await cache.GetOrSetAsync(
          HeroSnapshotCaching.SingleActiveKey(snapshot.HeroId),
          _ => Task.FromResult(snapshot),
          options: null,
          cancellationToken);
    }

    // Warm the active-version modifier catalogues used by hero/skill views.
    await dispatcher.SendAsync(
        new GetSkillModifiersByGameVersionQuery(),
        cancellationToken);

    await dispatcher.SendAsync(
        new GetActiveHeroModifiersQuery(),
        cancellationToken);

    logger.LogInformation(
        "HeroService cache warm-up completed. Skills: {SkillCount}; heroes: {HeroCount}; snapshots: {SnapshotCount}.",
        skills.Count,
        heroes.Count,
        snapshots.Count);
  }
}

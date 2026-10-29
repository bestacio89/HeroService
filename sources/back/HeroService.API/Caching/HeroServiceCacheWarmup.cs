using Franz.Common.Mediator.Dispatchers;
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
    var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
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

    // Warm the complete snapshot projection first, then the per-hero keys used
    // by individual snapshot requests. Each handler retains its own key policy.
    var snapshots = await dispatcher.SendAsync(
        new BrowseHeroSnapshotsQuery(),
        cancellationToken);

    foreach (var hero in heroes)
    {
      cancellationToken.ThrowIfCancellationRequested();

      await dispatcher.SendAsync(
          new GetHeroSnapshotQuery(hero.Id),
          cancellationToken);
    }

    // These bulk modifier reads are part of the skill/hero data used by the
    // client and are cached by the mediator query pipeline.
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

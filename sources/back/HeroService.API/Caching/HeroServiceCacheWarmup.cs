using System.Diagnostics;
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
/// Warms the primary catalogues and active-version snapshot projections before
/// the API begins accepting requests.
/// </summary>
public static class HeroServiceCacheWarmup
{
  private const int SnapshotCacheWriteConcurrency = 4;

  public static async Task WarmAsync(
      IServiceProvider services,
      CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(services);

    var total = Stopwatch.StartNew();

    await using var scope = services.CreateAsyncScope();
    var scopedServices = scope.ServiceProvider;
    var dispatcher = scopedServices.GetRequiredService<IDispatcher>();
    var cacheProvider = scopedServices.GetRequiredService<ICacheProvider>();
    var cacheWriter = cacheProvider as ICacheWriter;
    var logger = scopedServices.GetRequiredService<ILoggerFactory>()
        .CreateLogger("HeroService.CacheWarmup");

    var stage = Stopwatch.StartNew();
    var skills = await dispatcher.SendAsync(
        new GetAllSkillsQuery(),
        cancellationToken);
    LogStage(logger, "skill catalogue", stage);

    stage.Restart();
    var heroes = await dispatcher.SendAsync(
        new BrowseHeroesQuery(null, null, null, null),
        cancellationToken);
    LogStage(logger, "hero catalogue", stage);

    stage.Restart();
    await dispatcher.SendAsync(
        new GetAllHeroClassesQuery(),
        cancellationToken);
    LogStage(logger, "hero class catalogue", stage);

    stage.Restart();
    var snapshots = await dispatcher.SendAsync(
        new BrowseHeroSnapshotsQuery(),
        cancellationToken);
    LogStage(logger, "active-version snapshot generation", stage);

    if (cacheWriter is null)
    {
      logger.LogWarning(
          "The configured cache provider does not expose {CacheWriter}; per-hero snapshot pre-seeding was skipped.",
          nameof(ICacheWriter));
    }
    else if (snapshots.Count > 0)
    {
      stage.Restart();

      await Parallel.ForEachAsync(
          snapshots,
          new ParallelOptions
          {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = SnapshotCacheWriteConcurrency
          },
          async (snapshot, ct) =>
          {
            var key = HeroSnapshotCaching.SingleVersionKey(
                snapshot.GameVersionId,
                snapshot.HeroId);

            await cacheWriter.SetAsync(
                key,
                snapshot,
                options: null,
                ct);
          });

      LogStage(logger, "per-hero snapshot cache writes", stage);
    }

    stage.Restart();
    await dispatcher.SendAsync(
        new GetSkillModifiersByGameVersionQuery(),
        cancellationToken);
    await dispatcher.SendAsync(
        new GetActiveHeroModifiersQuery(),
        cancellationToken);
    LogStage(logger, "active-version modifier catalogues", stage);

    logger.LogInformation(
        "HeroService cache warm-up completed in {ElapsedMilliseconds} ms. Skills: {SkillCount}; heroes: {HeroCount}; snapshots: {SnapshotCount}.",
        total.ElapsedMilliseconds,
        skills.Count,
        heroes.Count,
        snapshots.Count);
  }

  private static void LogStage(
      ILogger logger,
      string stageName,
      Stopwatch stopwatch)
  {
    logger.LogInformation(
        "HeroService cache warm-up stage {StageName} completed in {ElapsedMilliseconds} ms.",
        stageName,
        stopwatch.ElapsedMilliseconds);
  }
}

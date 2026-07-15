namespace HeroService.Application.Heroes.Versioned.Snapshotting;

/// <summary>
/// Centralizes cache keys for active Hero Snapshot reads.
///
/// Snapshots are resolved only against the currently active GameVersion.
/// Version selection is owned by IGameVersionRepository and the version
/// activation workflow.
///
/// Cache invalidation happens when a new GameVersion becomes active:
/// - previous snapshot projections are invalidated
/// - new snapshots are generated against the new modifiers
///
/// TTL/eviction policy is intentionally NOT defined here. Passing null to
/// ICacheProvider.GetOrSetAsync uses the global CacheOptions configuration.
/// </summary>
public static class HeroSnapshotCaching
{
  private const string Prefix = "herosnapshot";


  /// <summary>
  /// Cache key for a single hero snapshot
  /// resolved against the active game version.
  /// </summary>
  public static string SingleActiveKey(Guid heroId)
      => $"{Prefix}:active:{heroId}";



  /// <summary>
  /// Cache key for the complete active hero snapshot catalog.
  /// </summary>
  public static string BrowseActiveKey()
      => $"{Prefix}:active:all";



  /// <summary>
  /// Optional invalidation prefix.
  /// Useful when a new version becomes active and the entire
  /// snapshot cache must be cleared.
  /// </summary>
  public static string ActivePrefix()
      => $"{Prefix}:active";
}
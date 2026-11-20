namespace HeroService.Application.Heroes.Versioned.Snapshotting;

/// <summary>
/// Centralizes cache keys for hero snapshots. Version identity is part of every
/// key so snapshots from a previously active game version cannot be served
/// after a version transition.
/// </summary>
public static class HeroSnapshotCaching
{
  private const string Prefix = "herosnapshot";

  public static string SingleVersionKey(Guid gameVersionId, Guid heroId)
      => $"{Prefix}:version:{gameVersionId}:{heroId}";

  public static string BrowseVersionKey(Guid gameVersionId)
      => $"{Prefix}:version:{gameVersionId}:all";
}

using Franz.Common.Caching.Abstractions;
using Franz.Common.Mediator.Messages;

namespace HeroService.API.Caching;

/// <summary>
/// Applies mediator response caching to query requests only.
/// Snapshot queries retain their existing domain-specific cache keys.
/// </summary>
public sealed class HeroServiceQueryCacheMetadataProvider : ICacheMetadataProvider
{
  public bool ShouldCache(object request)
  {
    ArgumentNullException.ThrowIfNull(request);

    var requestType = request.GetType();

    if (requestType.Name is "GetHeroSnapshotQuery" or "BrowseHeroSnapshotsQuery")
      return false;

    return requestType
        .GetInterfaces()
        .Any(type =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(IQuery<>));
  }

  public TimeSpan? GetCustomTtl(object request) => null;
}

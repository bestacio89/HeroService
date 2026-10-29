using Franz.Common.Caching.Abstractions;
using Franz.Common.Mediator.Messages;

namespace HeroService.API.Caching;

/// <summary>
/// Applies mediator response caching to query requests only.
/// Snapshot projections and per-skill detail queries retain their existing
/// data-loading behavior rather than sharing cached entity details.
/// </summary>
public sealed class HeroServiceQueryCacheMetadataProvider : ICacheMetadataProvider
{
  public bool ShouldCache(object request)
  {
    ArgumentNullException.ThrowIfNull(request);

    var requestType = request.GetType();

    if (requestType.Name is
        "GetHeroSnapshotQuery" or
        "BrowseHeroSnapshotsQuery" or
        "GetSkillDetailsQuery")
      return false;

    return requestType
        .GetInterfaces()
        .Any(type =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(IQuery<>));
  }

  public TimeSpan? GetCustomTtl(object request) => null;
}

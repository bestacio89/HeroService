using HeroService.Domain.Heroes.Versioned.GameVersion;

namespace HeroService.Contracts.Persistence.GameVersions;

public interface IGameVersionRepository : INameLookupRepository<GameVersion, Guid>
{
  Task<GameVersion?> GetActiveAsync(CancellationToken cancellationToken = default);

  
}
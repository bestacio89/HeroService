using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class GameVersionRepository : IGameVersionRepository
{
  private readonly ApplicationDbContext _dbContext;

  public GameVersionRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<GameVersion?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<GameVersion>()
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.VersionName == name, cancellationToken);
  }

  public Task<GameVersion?> GetActiveAsync(
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<GameVersion>()
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.IsActive, cancellationToken);
  }
}
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class HeroModifierRepository : IHeroModifierRepository
{
  private readonly DbContext _dbContext;

  public HeroModifierRepository(DbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<HeroModifier?> GetAsync(
      Guid heroId,
      Guid gameVersionId,
      CancellationToken ct)
  {
    return await _dbContext.Set<HeroModifier>()
      .AsNoTracking()
      .FirstOrDefaultAsync(x =>
          x.HeroId == heroId &&
          x.GameVersionId == gameVersionId,
          ct);
  }

  public async Task<IReadOnlyDictionary<Guid, HeroModifier>> GetByHeroIdsAsync(
      IReadOnlyList<Guid> heroIds,
      Guid gameVersionId,
      CancellationToken ct)
  {
    var result = await _dbContext.Set<HeroModifier>()
      .AsNoTracking()
      .Where(x =>
          heroIds.Contains(x.HeroId) &&
          x.GameVersionId == gameVersionId)
      .ToListAsync(ct);

    return result.ToDictionary(x => x.HeroId, x => x);
  }
}
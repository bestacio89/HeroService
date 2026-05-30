using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Persistence;

public sealed class HeroModifierRepository : IHeroModifierRepository
{
  private readonly ApplicationDbContext _db;

  public HeroModifierRepository(ApplicationDbContext db)
  {
    _db = db;
  }

  public Task<HeroModifier?> GetByHeroAndVersionAsync(
      Guid heroId,
      Guid gameVersionId,
      CancellationToken cancellationToken = default)
  {
    return _db.Set<HeroModifier>()
        .AsNoTracking()
        .FirstOrDefaultAsync(
            x => x.HeroId == heroId &&
                 x.GameVersionId == gameVersionId,
            cancellationToken);
  }

  public async Task<IReadOnlyList<HeroModifier>> GetByGameVersionIdAsync(
      Guid gameVersionId,
      CancellationToken cancellationToken = default)
  {
    return await _db.Set<HeroModifier>()
        .AsNoTracking()
        .Where(x => x.GameVersionId == gameVersionId)
        .ToListAsync(cancellationToken);
  }
}
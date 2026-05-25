using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Core;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class HeroRepository : IHeroRepository
{
  private readonly ApplicationDbContext _dbContext;

  public HeroRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  // =========================================================
  // BASE QUERY (CENTRALIZED GRAPH LOADING)
  // =========================================================
  private IQueryable<Hero> BaseQuery()
  {
    return _dbContext.Heroes
        .Include(x => x.HeroClass)
        .Include(x => x.Affiliation)
            .ThenInclude(x => x.Archetype)
        .Include(x => x.Affiliation)
            .ThenInclude(x => x.Mythology)
        .Include(x => x.Affiliation)
            .ThenInclude(x => x.OriginCulture)
        .Include(x => x.BaseStats)
        .Include(x => x.SkillKit);
  }

  // =========================================================
  // NAME LOOKUP (from INameLookupRepository)
  // =========================================================
  public async Task<Hero?> GetByNameAsync(
     string name,
     CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<Hero>()
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Name == name,
        cancellationToken);
  }

  // =========================================================
  // SINGLE AGGREGATE READ
  // =========================================================
  public Task<Hero?> GetByIdAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Heroes
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
  }

  public Task<Hero?> GetByIdWithDetailsAsync(
      Guid id,
      CancellationToken cancellationToken = default)
  {
    return BaseQuery()
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
  }

  public Task<Hero?> GetByNameWithDetailsAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return BaseQuery()
        .FirstOrDefaultAsync(x => x.Name == name, cancellationToken);
  }

  // =========================================================
  // FILTERED READS
  // =========================================================
  public async Task<IReadOnlyCollection<Hero>> GetByClassAsync(
      Guid heroClassId,
      CancellationToken cancellationToken = default)
  {
    return await BaseQuery()
        .Where(x => x.HeroClassId == heroClassId)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Hero>> GetByMythologyAsync(
      Guid mythologyTypeId,
      CancellationToken cancellationToken = default)
  {
    return await BaseQuery()
        .Where(x => x.Affiliation.Mythology.Id == mythologyTypeId)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Hero>> GetByCultureAsync(
      Guid cultureId,
      CancellationToken cancellationToken = default)
  {
    return await BaseQuery()
        .Where(x => x.Affiliation.OriginCulture.Id == cultureId)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Hero>> GetByArchetypeAsync(
      Guid archetypeId,
      CancellationToken cancellationToken = default)
  {
    return await BaseQuery()
        .Where(x => x.Affiliation.Archetype.Id == archetypeId)
        .ToListAsync(cancellationToken);
  }

  public Task<Hero?> GetDetailsAsync(
    Guid heroId,
    CancellationToken cancellationToken = default)
  {
    return BaseQuery()
        .FirstOrDefaultAsync(x => x.Id == heroId, cancellationToken);
  }

  // =========================================================
  // FULL BROWSE
  // =========================================================
  public async Task<IReadOnlyCollection<Hero>> GetAllWithDetailsAsync(
      CancellationToken cancellationToken = default)
  {
    return await BaseQuery()
        .ToListAsync(cancellationToken);
  }
}
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class SkillRepository : ISkillRepository
{
  private readonly ApplicationDbContext _dbContext;

  public SkillRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public Task<Skill?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<Skill>()
      .AsNoTracking()
      .FirstOrDefaultAsync(
          x => x.Name == name,
          cancellationToken);
  }

  // =========================================================
  // NEW: BATCH SNAPSHOT SUPPORT
  // =========================================================
  public async Task<IReadOnlyList<Skill>> GetByIdsAsync(
      IEnumerable<Guid> ids,
      CancellationToken cancellationToken = default)
  {
    if (ids is null)
      throw new ArgumentNullException(nameof(ids));

    var idList = ids
      .Where(id => id != Guid.Empty)
      .Distinct()
      .ToList();

    if (idList.Count == 0)
      return Array.Empty<Skill>();

    return await _dbContext.Set<Skill>()
      .AsNoTracking()
      .Include(s => s.Effects) // IMPORTANT for snapshot correctness
      .Where(s => idList.Contains(s.Id))
      .ToListAsync(cancellationToken);
  }
}
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

  public async Task<IReadOnlyList<Skill>> GetByIdsAsync(
      IEnumerable<Guid> ids,
      CancellationToken cancellationToken = default)
  {
    if (ids is null)
      throw new ArgumentNullException(nameof(ids));

    var idList = ids
        .Where(x => x != Guid.Empty)
        .Distinct()
        .ToList();

    if (idList.Count == 0)
      return Array.Empty<Skill>();

    return await _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(x => x.Effects)
        .Where(x => idList.Contains(x.Id))
        .ToListAsync(cancellationToken);
  }

  public Task<Skill?> GetDetailsAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(x => x.Effects)
        .FirstOrDefaultAsync(
            x => x.Id == skillId,
            cancellationToken);
  }

  public Task<Skill?> GetByNameWithDetailsAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(x => x.Effects)
        .FirstOrDefaultAsync(
            x => x.Name == name,
            cancellationToken);
  }

  public async Task<IReadOnlyCollection<Skill>> GetAllWithDetailsAsync(
      CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(x => x.Effects)
        .OrderBy(x => x.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Skill>> GetByTypeAsync(
      SkillType skillType,
      CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(x => x.Effects)
        .Where(x => x.SkillType == skillType)
        .OrderBy(x => x.Name)
        .ToListAsync(cancellationToken);
  }
}
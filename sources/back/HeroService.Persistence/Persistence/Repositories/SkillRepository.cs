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
    ArgumentNullException.ThrowIfNull(ids);

    var idList = ids
        .Where(id => id != Guid.Empty)
        .Distinct()
        .ToArray();

    if (idList.Length == 0)
      return Array.Empty<Skill>();

    return await _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(skill => skill.Effects)
        .Where(skill => idList.Contains(skill.Id))
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyList<SkillLookup>> GetLookupsByIdsAsync(
      IEnumerable<Guid> ids,
      CancellationToken cancellationToken = default)
  {
    ArgumentNullException.ThrowIfNull(ids);

    var idList = ids
        .Where(id => id != Guid.Empty)
        .Distinct()
        .ToArray();

    if (idList.Length == 0)
      return Array.Empty<SkillLookup>();

    return await _dbContext.Set<Skill>()
        .AsNoTracking()
        .Where(skill => idList.Contains(skill.Id))
        .OrderBy(skill => skill.Name)
        .Select(skill => new SkillLookup(skill.Id, skill.Name))
        .ToListAsync(cancellationToken);
  }

  public Task<Skill?> GetDetailsAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(skill => skill.Effects)
        .Include(skill => skill.BaseStats)
        .FirstOrDefaultAsync(
            skill => skill.Id == skillId,
            cancellationToken);
  }

  public Task<Skill?> GetByNameWithDetailsAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(skill => skill.Effects)
        .Include(skill => skill.BaseStats)
        .FirstOrDefaultAsync(
            skill => skill.Name == name,
            cancellationToken);
  }

  public async Task<IReadOnlyCollection<Skill>> GetAllWithDetailsAsync(
      CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(skill => skill.Effects)
        .Include(skill => skill.BaseStats)
        .OrderBy(skill => skill.Name)
        .ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyCollection<Skill>> GetByTypeAsync(
      SkillType skillType,
      CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<Skill>()
        .AsNoTracking()
        .Include(skill => skill.Effects)
        .Where(skill => skill.SkillType == skillType)
        .OrderBy(skill => skill.Name)
        .ToListAsync(cancellationToken);
  }

  public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
  {
    return _dbContext.Skills.AnyAsync(
        skill => skill.Name == name,
        cancellationToken);
  }
}

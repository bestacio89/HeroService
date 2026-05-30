using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class SkillLoreRepository : ISkillLoreRepository
{
  private readonly ApplicationDbContext _dbContext;

  public SkillLoreRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }



  public async Task<SkillLore?> GetLoreBySkillNameAsync(
    string skillName,
    CancellationToken cancellationToken = default)
  {
    var skill = await _dbContext.Set<Skill>()
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Name == skillName, cancellationToken);

    if (skill is null)
      return null;

    return await _dbContext.Set<SkillLore>()
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.SkillId == skill.Id, cancellationToken);
  }

  public Task<SkillLore?> GetBySkillIdAsync(
      Guid skillId,
      CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<SkillLore>()
        .AsNoTracking()
        .FirstOrDefaultAsync(
            x => x.SkillId == skillId,
            cancellationToken);
  }
}
using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class SkillBaseStatsRepository : ISkillBaseStatsRepository
{
  private readonly ApplicationDbContext _dbContext;

  public SkillBaseStatsRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<IReadOnlyDictionary<Guid, SkillBaseStats>> GetBySkillIdsAsync(
      IReadOnlyList<Guid> skillIds,
      CancellationToken ct)
  {
    return await _dbContext.Set<SkillBaseStats>()
      .AsNoTracking()
      .Where(x => skillIds.Contains(x.SkillId))
      .ToDictionaryAsync(x => x.SkillId, ct);
  }
}
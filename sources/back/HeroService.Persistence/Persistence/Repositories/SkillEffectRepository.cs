using HeroService.Contracts.Persistence.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class SkillEffectRepository : ISkillEffectRepository
{
  private readonly ApplicationDbContext _dbContext;

  public SkillEffectRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<IReadOnlyDictionary<Guid, List<SkillEffect>>> GetBySkillIdsAsync(
      IReadOnlyList<Guid> skillIds,
      CancellationToken ct)
  {
    return await _dbContext.Set<SkillEffect>()
      .AsNoTracking()
      .Where(x => skillIds.Contains(x.SkillId))
      .GroupBy(x => x.SkillId)
      .ToDictionaryAsync(
          g => g.Key,
          g => g.ToList(),
          ct);
  }
}
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class SkillModifierRepository : ISkillModifierRepository
{
  private readonly DbContext _dbContext;

  public SkillModifierRepository(DbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<IReadOnlyDictionary<Guid, SkillModifier?>> GetBySkillIdsAsync(
      IReadOnlyList<Guid> skillIds,
      CancellationToken ct)
  {
    var result = await _dbContext.Set<SkillModifier>()
      .AsNoTracking()
      .Where(x => skillIds.Contains(x.SkillId))
      .ToListAsync(ct);

    return result.ToDictionary(x => x.SkillId, x => (SkillModifier?)x);
  }
}
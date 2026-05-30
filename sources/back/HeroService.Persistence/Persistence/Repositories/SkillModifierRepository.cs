using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class SkillModifierRepository : ISkillModifierRepository
{
  private readonly ApplicationDbContext _db;

  public SkillModifierRepository(ApplicationDbContext db)
  {
    _db = db;
  }

  public Task<SkillModifier?> GetBySkillAndVersionAsync(
      Guid skillId,
      Guid gameVersionId,
      CancellationToken cancellationToken = default)
  {
    return _db.Set<SkillModifier>()
        .AsNoTracking()
        .FirstOrDefaultAsync(
            x => x.SkillId == skillId &&
                 x.GameVersionId == gameVersionId,
            cancellationToken);
  }

  public async Task<IReadOnlyList<SkillModifier>> GetByGameVersionIdAsync(
      Guid gameVersionId,
      CancellationToken cancellationToken = default)
  {
    return await _db.Set<SkillModifier>()
        .AsNoTracking()
        .Where(x => x.GameVersionId == gameVersionId)
        .ToListAsync(cancellationToken);
  }

  // Batch resolver
  public async Task<IReadOnlyList<SkillModifier>> GetBySkillIdsAndVersionAsync(
      IReadOnlyCollection<Guid> skillIds,
      Guid gameVersionId,
      CancellationToken cancellationToken = default)
  {
    if (skillIds == null || skillIds.Count == 0)
      return Array.Empty<SkillModifier>();

    return await _db.SkillModifiers
        .AsNoTracking()
        .Where(x =>
            x.GameVersionId == gameVersionId &&
            skillIds.Contains(x.SkillId))
        .ToListAsync(cancellationToken);
  }
}
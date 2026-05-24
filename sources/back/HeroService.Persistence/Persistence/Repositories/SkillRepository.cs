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
}
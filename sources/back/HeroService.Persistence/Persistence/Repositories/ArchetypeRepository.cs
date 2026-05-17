using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using Microsoft.EntityFrameworkCore;
#nullable enable
namespace HeroService.Persistence.Repositories;

public sealed class ArchetypeRepository : IArchetypeRepository
{
  private readonly ApplicationDbContext dbContext;

  public ArchetypeRepository(ApplicationDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<OriginArchetype?> GetByNameAsync(
    string name,
    CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<OriginArchetype>()
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x. == name,
        cancellationToken);
  }
}
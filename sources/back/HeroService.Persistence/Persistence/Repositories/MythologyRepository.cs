using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using Microsoft.EntityFrameworkCore;
#nullable enable
namespace HeroService.Persistence.Persistence.Repositories;

public sealed class MythologyRepository : IMythologyRepository
{
  private readonly ApplicationDbContext _dbContext;

  public MythologyRepository(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<MythologyType?> GetByNameAsync(
    string name,
    CancellationToken cancellationToken = default)
  {
    return await _dbContext.Set<MythologyType>()
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Name == name,
        cancellationToken);
  }
}
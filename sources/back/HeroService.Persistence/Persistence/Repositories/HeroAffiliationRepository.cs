using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class MythologyRepository
    :  IMythologyRepository
{
  public MythologyRepository(ApplicationDbContext dbContext)
      
  {
  }

  public async Task<MythologyType?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default)
  {
    return await DbContext.Set<MythologyType>()
        .AsNoTracking()
        .FirstOrDefaultAsync(
            x => x.Name.ToLower() == name.ToLower(),
            cancellationToken);
  }
}
using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Persistence.Persistence.Repositories;

internal class OriginCultureRepository : ICultureRepository
{
  private readonly ApplicationDbContext dbContext;

  public OriginCultureRepository(ApplicationDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<OriginCulture?> GetByNameAsync(
    string name,
    CancellationToken cancellationToken = default)
  {
    return await dbContext.Set<OriginCulture>()
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Name == name,
        cancellationToken);
  }
}

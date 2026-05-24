using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Domain.Heroes.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Persistence.Persistence.Repositories;

public class HeroClassRepository : IHeroClassRepository
{
  private readonly ApplicationDbContext dbContext;

  public HeroClassRepository(ApplicationDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<HeroClass?> GetByNameAsync(
    string name,
    CancellationToken cancellationToken = default)
  {
    return await dbContext.Set<HeroClass>()
      .AsNoTracking()
      .FirstOrDefaultAsync(
        x => x.Name == name,
        cancellationToken);
  }
}

using HeroService.Contracts.Persistence;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using Microsoft.EntityFrameworkCore;
#nullable enable
namespace HeroService.Persistence.Repositories;

public sealed class HeroAffiliationRepository : IHeroAffiliationRepository
{
  private readonly ApplicationDbContext dbContext;

  public HeroAffiliationRepository(ApplicationDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<HeroAffiliation?> GetByOriginAndMythologyAsync(
      OriginType originType,
      Guid mythologyTypeId,
      CancellationToken cancellationToken = default)
  {
    return await dbContext.Set<HeroAffiliation>()
        .AsNoTracking()
        .FirstOrDefaultAsync(
            x => x.OriginType == originType &&
                 x.MythologyTypeId == mythologyTypeId,
            cancellationToken);
  }
}
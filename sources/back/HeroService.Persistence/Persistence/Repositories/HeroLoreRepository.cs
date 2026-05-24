using Franz.Common.Business.Repositories;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Domain.Heroes.Core;
using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public sealed class HeroLoreRepository : IHeroLoreRepository
{
  private readonly IEntityRepository<HeroLore, Guid> _repository;
  private readonly DbContext _dbContext;

  public HeroLoreRepository(
      IEntityRepository<HeroLore, Guid> repository,
      DbContext dbContext)
  {
    _repository = repository;
    _dbContext = dbContext;
  }

  public Task<HeroLore?> GetByHeroIdAsync(Guid heroId, CancellationToken cancellationToken = default)
  {
    return _dbContext.Set<HeroLore>()
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.HeroId == heroId, cancellationToken);
  }
}
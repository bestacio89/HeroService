using Microsoft.EntityFrameworkCore;

namespace HeroService.Persistence.Persistence.Repositories;

public abstract class RepositoryBase<TDbContext>
    where TDbContext : DbContext
{
  protected readonly TDbContext DbContext;

  protected RepositoryBase(TDbContext dbContext)
  {
    DbContext = dbContext;
  }
}
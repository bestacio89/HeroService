using Franz.Common.DependencyInjection;

namespace HeroService.Contracts.Persistence;

public interface INameLookupRepository<TEntity, TId> : IScopedDependency
{
  Task<TEntity?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);
}
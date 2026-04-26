using Franz.Common.DependencyInjection;
using HeroService.Domain.Heroes.Affiliations;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Contracts.Persistence;

public interface IHeroAffiliationRepository : IScopedDependency
{
  Task<HeroAffiliation?> GetByOriginAndMythologyAsync(
      OriginType originType,
      Guid mythologyTypeId,
      CancellationToken cancellationToken = default);
}
using Franz.Common.DependencyInjection;
using HeroService.Contracts.Queries.Heroes.Classes;
using HeroService.Domain.Heroes.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence.Heroes;

public interface IHeroLoreRepository : IScopedDependency
{
  Task<HeroLore?> GetByHeroIdAsync(Guid heroId, CancellationToken cancellationToken = default);
}

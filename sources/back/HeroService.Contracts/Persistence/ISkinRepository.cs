using HeroService.Domain.Skins;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence;



public interface ISkinRepository :
    INameLookupRepository<Skin, Guid>
{
  Task<IReadOnlyList<Skin>> GetByHeroIdAsync(
      Guid heroId,
      CancellationToken cancellationToken = default);
}

using HeroService.Domain.Heroes.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence.Heroes;

public interface IHeroRepository : INameLookupRepository<Hero, Guid>
{
}

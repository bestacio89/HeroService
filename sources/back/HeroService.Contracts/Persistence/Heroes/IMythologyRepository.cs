using HeroService.Domain.Heroes.Affiliations.Classifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence.Heroes;


public interface IMythologyRepository :
    INameLookupRepository<MythologyType, Guid>
{
}

using HeroService.Domain.Heroes.Affiliations.Classifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Persistence;

public interface IArchetypeRepository :INameLookupRepository<OriginArchetype,Guid>
{
}

using HeroService.Domain.Heroes.Affiliations.Classifications;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HeroService.Contracts.Persistence.Heroes;

public interface IOriginCultureRepository: INameLookupRepository<OriginCulture, Guid>
{

}

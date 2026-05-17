using HeroService.Domain.Heroes.Affiliations.Classifications;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HeroService.Contracts.Persistence;

public interface ICultureRepository: INameLookupRepository<OriginCulture, Guid>
{

}

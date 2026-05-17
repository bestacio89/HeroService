using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs;
using HeroService.Contracts.DTOs.Heroes.Affiliations;
using System.Collections.Generic;

namespace HeroService.Contracts.Queries.Heroes;

public sealed record GetMythologyTypesQuery()
    : IQuery<IEnumerable<MythologyTypeDto>>;
using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Heroes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Queries.Heroes.Classification.OriginArchetype;

public sealed record GetAllOriginArchetypesQuery()
    : IQuery<IReadOnlyList<OriginArchetypeDto>>;

using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Heroes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Queries.Heroes.Classification.OriginCultures;

public sealed record GetAllOriginCulturesQuery()
    : IQuery<IReadOnlyList<OriginCultureDto>>;

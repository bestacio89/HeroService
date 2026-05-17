using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs;

namespace HeroService.Contracts.Queries.Heroes;

public sealed record GetHeroAffiliationByHeroIdQuery(
    Guid HeroId
) : IQuery<HeroAffiliationDto?>;
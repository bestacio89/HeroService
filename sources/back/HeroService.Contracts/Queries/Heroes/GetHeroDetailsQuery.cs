using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Contracts.Queries.Heroes;

public sealed record GetHeroDetailsQuery(
    Guid HeroId
) : IQuery<HeroDto>;
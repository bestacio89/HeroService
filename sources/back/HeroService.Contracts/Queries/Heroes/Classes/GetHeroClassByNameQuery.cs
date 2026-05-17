using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Contracts.Queries.Heroes.Classes;

public sealed record GetHeroClassByNameQuery(
    string Name
) : IQuery<HeroClassDto>;
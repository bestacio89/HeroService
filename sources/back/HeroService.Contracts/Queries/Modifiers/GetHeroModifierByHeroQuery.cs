using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Modifiers;

namespace HeroService.Contracts.Queries.Modifiers;

public sealed record GetHeroModifierByHeroQuery(
    Guid HeroId
) : IQuery<HeroModifierDto>;
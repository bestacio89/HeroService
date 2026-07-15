using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Modifiers;

public sealed record GetActiveHeroModifiersQuery(

) : IQuery<IReadOnlyList<HeroModifierDto>>;
using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Modifiers;

namespace HeroService.Contracts.Queries.Modifiers;

public sealed record GetHeroModifiersByGameVersionQuery(
	Guid GameVersionId
) : IQuery<IReadOnlyList<HeroModifierDto>>;
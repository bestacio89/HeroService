using Franz.Common.Mediator.Messages;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Contracts.Commands.Heroes;

public sealed record UpdateHeroAffiliationCommand(
    Guid HeroId,
    OriginArchetype OriginType,
    Guid MythologyTypeId
) : ICommand;
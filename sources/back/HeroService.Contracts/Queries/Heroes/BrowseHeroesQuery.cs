using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Heroes;

namespace HeroService.Contracts.Queries.Heroes;

public sealed record BrowseHeroesQuery(
    Guid? HeroClassId,
    Guid? MythologyTypeId,
    Guid? CultureId,
    Guid? ArchetypeId
) : IQuery<IReadOnlyCollection<HeroDetailsDto>>;
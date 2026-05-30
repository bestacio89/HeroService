using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Queries.Heroes;
using HeroService.Domain.Heroes.Core;

namespace HeroService.Application.Queries.Heroes;

public sealed class BrowseHeroesQueryHandler
    : IQueryHandler<BrowseHeroesQuery, IReadOnlyCollection<HeroDto>>
{
  private readonly IHeroRepository _repository;
  private readonly IFranzMapper _mapper;

  public BrowseHeroesQueryHandler(
      IHeroRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyCollection<HeroDto>> Handle(
      BrowseHeroesQuery query,
      CancellationToken cancellationToken)
  {
    var heroes = await _repository.BrowseAsync(
        query.HeroClassId,
        query.MythologyTypeId,
        query.CultureId,
        query.ArchetypeId,
        cancellationToken);

    return _mapper.Map<IReadOnlyCollection<Hero>, IReadOnlyCollection<HeroDto>>(heroes);
  }
}
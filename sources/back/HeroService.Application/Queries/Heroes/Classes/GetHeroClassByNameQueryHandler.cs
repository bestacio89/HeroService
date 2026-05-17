using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Persistence;
using HeroService.Contracts.Queries.Heroes.Classes;
using HeroService.Domain.Heroes.Core;

namespace HeroService.Application.Queries.Heroes.Classes;

public sealed class GetHeroClassByNameQueryHandler
    : IQueryHandler<GetHeroClassByNameQuery, HeroClassDto?>
{
  private readonly IHeroClassRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetHeroClassByNameQueryHandler(
      IHeroClassRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<HeroClassDto?> Handle(
      GetHeroClassByNameQuery query,
      CancellationToken cancellationToken)
  {
    var heroClass = await _repository.GetByNameAsync(query.Name, cancellationToken);

    if (heroClass is null)
      return null;

    return _mapper.Map<HeroClass, HeroClassDto>(heroClass);
  }
}
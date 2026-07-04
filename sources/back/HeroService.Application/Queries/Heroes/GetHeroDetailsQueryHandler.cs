using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Queries.Heroes;
using HeroService.Domain.Heroes.Core;

namespace HeroService.Application.Queries.Heroes;

public sealed class GetHeroDetailsQueryHandler
    : IQueryHandler<GetHeroDetailsQuery, HeroDto>
{
  private readonly IHeroRepository _heroRepository;
  private readonly IFranzMapper _mapper;

  public GetHeroDetailsQueryHandler(
      IHeroRepository heroRepository,
      IFranzMapper mapper)
  {
    _heroRepository = heroRepository;
    _mapper = mapper;
  }

  public async Task<HeroDto> Handle(
      GetHeroDetailsQuery request,
      CancellationToken cancellationToken)
  {
    var hero = await _heroRepository.GetDetailsAsync(
        request.HeroId,
        cancellationToken);

    if (hero is null)
      throw new KeyNotFoundException($"Hero '{request.HeroId}' not found.");

    return _mapper.Map<Hero, HeroDto>(hero);
  }
}
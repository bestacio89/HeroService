using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Messages;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Queries.Heroes;
using HeroService.Domain.Heroes.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Application.Queries.Heroes;

public sealed class GetHeroByNameQueryHandler : IQueryHandler<GetHeroByNameQuery, HeroDetailsDto>

{
  private readonly IHeroRepository _heroRepository;
  private readonly IFranzMapper _mapper;

  public GetHeroByNameQueryHandler(
      IHeroRepository heroRepository,
      IFranzMapper mapper)
  {
    _heroRepository = heroRepository;
    _mapper = mapper;
  }

  public async Task<HeroDetailsDto> Handle(
      GetHeroByNameQuery request,
      CancellationToken cancellationToken)
  {
    var hero = await _heroRepository.GetByNameWithDetailsAsync(
        request.Name,
        cancellationToken);

    if (hero is null)
      throw new KeyNotFoundException($"Hero '{request.Name}' not found.");

    return _mapper.Map<Hero, HeroDetailsDto>(hero);
  }
}

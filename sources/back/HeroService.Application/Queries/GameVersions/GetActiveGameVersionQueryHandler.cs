using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Queries.GameVersions;
using HeroService.Domain.Heroes.Versioned.GameVersion;

namespace HeroService.Application.Queries.GameVersions;

public sealed class GetActiveGameVersionQueryHandler
    : IQueryHandler<GetActiveGameVersionQuery, GameVersionDto?>
{
  private readonly IGameVersionRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetActiveGameVersionQueryHandler(
      IGameVersionRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<GameVersionDto?> Handle(
      GetActiveGameVersionQuery request,
      CancellationToken cancellationToken)
  {
    var active = await _repository.GetActiveAsync(cancellationToken);

    return active is null
        ? null
        : _mapper.Map<GameVersion, GameVersionDto>(active);
  }
}
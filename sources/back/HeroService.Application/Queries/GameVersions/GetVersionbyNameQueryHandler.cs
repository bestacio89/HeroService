using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Queries.GameVersions;
using HeroService.Domain.Heroes.Versioned.GameVersion;

public sealed class GetGameVersionByNameQueryHandler
    : IQueryHandler<GetGameVersionByNameQuery, GameVersionDto>
{
  private readonly IGameVersionRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetGameVersionByNameQueryHandler(
      IGameVersionRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<GameVersionDto> Handle(
      GetGameVersionByNameQuery request,
      CancellationToken cancellationToken)
  {
    var version = await _repository.GetByNameAsync(
        request.Name,
        cancellationToken);

    if (version is null)
      throw new InvalidOperationException("GameVersion not found.");

    return _mapper.Map<GameVersion, GameVersionDto>(version);
  }
}
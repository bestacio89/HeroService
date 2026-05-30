using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Contracts.Queries.GameVersions;
using HeroService.Domain.Heroes.Versioned.GameVersion;

public sealed class GetGameVersionByIdQueryHandler
    : IQueryHandler<GetGameVersionByIdQuery, GameVersionDto>
{
  private readonly IEntityRepository<GameVersion, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetGameVersionByIdQueryHandler(
      IEntityRepository<GameVersion, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<GameVersionDto> Handle(
      GetGameVersionByIdQuery request,
      CancellationToken cancellationToken)
  {
    var version = await _repository.GetByIdAsync(
        request.Id,
        cancellationToken);

    if (version is null)
      throw new InvalidOperationException("GameVersion not found.");

    return _mapper.Map<GameVersion, GameVersionDto>(version);
  }
}
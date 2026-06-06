using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mapping.Abstractions;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Contracts.Queries.GameVersions;

namespace HeroService.Application.Queries.GameVersions;

public sealed class GetAllGameVersionsQueryHandler
    : IQueryHandler<GetAllGameVersionsQuery, IReadOnlyCollection<GameVersionDto>>
{
  private readonly IEntityRepository<GameVersion, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetAllGameVersionsQueryHandler(
      IEntityRepository<GameVersion, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }
  

  public async Task<IReadOnlyCollection<GameVersionDto>> Handle(
      GetAllGameVersionsQuery request,
      CancellationToken cancellationToken)
  {
    var versions = await _repository.GetAllAsync(cancellationToken);

    return _mapper.Map<IReadOnlyCollection<GameVersion>, IReadOnlyCollection<GameVersionDto>>(versions);
  }
}
using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.MythologyTypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Queries.Heroes.Classification.MythologyTypes;

public sealed class GetAllMythologyTypesQueryHandler
    : IQueryHandler<GetAllMythologyTypesQuery, IReadOnlyList<MythologyTypeDto>>
{
  private readonly IEntityRepository<MythologyType, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetAllMythologyTypesQueryHandler(
      IEntityRepository<MythologyType, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<MythologyTypeDto>> Handle(
      GetAllMythologyTypesQuery query,
      CancellationToken cancellationToken)
  {
    var entities = await _repository.GetAllAsync(cancellationToken);

    return _mapper.Map<IReadOnlyList<MythologyType>, IReadOnlyList<MythologyTypeDto>>(entities);
  }
}
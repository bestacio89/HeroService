using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.OriginArchetypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Queries.Heroes.Classification.OriginArchetypes;

public sealed class GetAllOriginArchetypesQueryHandler
    : IQueryHandler<GetAllOriginArchetypesQuery, IReadOnlyList<OriginArchetypeDto>>
{
  private readonly IEntityRepository<OriginArchetype, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetAllOriginArchetypesQueryHandler(
      IEntityRepository<OriginArchetype, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<OriginArchetypeDto>> Handle(
      GetAllOriginArchetypesQuery query,
      CancellationToken cancellationToken)
  {
    var entities = await _repository.GetAllAsync(cancellationToken);

    return _mapper.Map<IReadOnlyList<OriginArchetype>, IReadOnlyList<OriginArchetypeDto>>(entities);
  }
}
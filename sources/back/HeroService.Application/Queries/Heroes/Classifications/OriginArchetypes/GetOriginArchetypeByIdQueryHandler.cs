using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.OriginArchetypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Queries.Heroes.Classification.OriginArchetypes;

public sealed class GetOriginArchetypeByIdQueryHandler
    : IQueryHandler<GetOriginArchetypeByIdQuery, OriginArchetypeDto?>
{
  private readonly IEntityRepository<OriginArchetype, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetOriginArchetypeByIdQueryHandler(
      IEntityRepository<OriginArchetype, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<OriginArchetypeDto?> Handle(
      GetOriginArchetypeByIdQuery query,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(query.Id, cancellationToken);

    if (entity is null)
      return null;

    return _mapper.Map<OriginArchetype, OriginArchetypeDto>(entity);
  }
}
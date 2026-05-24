using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.OriginArchetypes;
using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Contracts.Persistence.Heroes;

namespace HeroService.Application.Queries.Heroes.Classification.OriginArchetypes;

public sealed class GetOriginArchetypeByNameQueryHandler
    : IQueryHandler<GetOriginArchetypeByNameQuery, OriginArchetypeDto?>
{
  private readonly IArchetypeRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetOriginArchetypeByNameQueryHandler(
      IArchetypeRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<OriginArchetypeDto?> Handle(
      GetOriginArchetypeByNameQuery query,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByNameAsync(query.Name, cancellationToken);

    if (entity is null)
      return null;

    return _mapper.Map<OriginArchetype, OriginArchetypeDto>(entity);
  }
}
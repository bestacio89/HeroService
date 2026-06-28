using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.OriginCultures;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Queries.Heroes.Classification.OriginCultures;

public sealed class GetOriginCultureByNameQueryHandler
    : IQueryHandler<GetOriginCultureByNameQuery, OriginCultureDto?>
{
  private readonly IOriginCultureRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetOriginCultureByNameQueryHandler(
      IOriginCultureRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<OriginCultureDto?> Handle(
      GetOriginCultureByNameQuery query,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByNameAsync(query.Name, cancellationToken);

    if (entity is null)
      return null;

    return _mapper.Map<OriginCulture, OriginCultureDto>(entity);
  }
}
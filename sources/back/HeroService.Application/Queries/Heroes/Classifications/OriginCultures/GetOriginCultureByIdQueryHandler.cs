using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.OriginCultures;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Queries.Heroes.Classification.OriginCultures;

public sealed class GetOriginCultureByIdQueryHandler
    : IQueryHandler<GetOriginCultureByIdQuery, OriginCultureDto?>
{
  private readonly IEntityRepository<OriginCulture, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetOriginCultureByIdQueryHandler(
      IEntityRepository<OriginCulture, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<OriginCultureDto?> Handle(
      GetOriginCultureByIdQuery query,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(query.Id, cancellationToken);

    if (entity is null)
      return null;

    return _mapper.Map<OriginCulture, OriginCultureDto>(entity);
  }
}
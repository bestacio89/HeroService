using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.OriginCultures;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Queries.Heroes.Classification.OriginCultures;

public sealed class GetAllOriginCulturesQueryHandler
    : IQueryHandler<GetAllOriginCulturesQuery, IReadOnlyList<OriginCultureDto>>
{
  private readonly IEntityRepository<OriginCulture, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetAllOriginCulturesQueryHandler(
      IEntityRepository<OriginCulture, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<OriginCultureDto>> Handle(
      GetAllOriginCulturesQuery query,
      CancellationToken cancellationToken)
  {
    var entities = await _repository.GetAllAsync(cancellationToken);

    return _mapper.Map<IReadOnlyList<OriginCulture>, IReadOnlyList<OriginCultureDto>>(entities);
  }
}
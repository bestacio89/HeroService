using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.MythologyType;
using HeroService.Domain.Heroes.Affiliations.Classifications;

namespace HeroService.Application.Queries.Heroes.Classification.MythologyTypes;

public sealed class GetMythologyTypeByIdQueryHandler
    : IQueryHandler<GetMythologyTypeByIdQuery, MythologyTypeDto?>
{
  private readonly IEntityRepository<MythologyType, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetMythologyTypeByIdQueryHandler(
      IEntityRepository<MythologyType, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<MythologyTypeDto?> Handle(
      GetMythologyTypeByIdQuery query,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(query.Id, cancellationToken);

    if (entity is null)
      return null;

    return _mapper.Map<MythologyType, MythologyTypeDto>(entity);
  }
}
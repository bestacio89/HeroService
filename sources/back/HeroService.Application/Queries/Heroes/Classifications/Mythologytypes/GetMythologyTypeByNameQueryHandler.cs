using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;

using HeroService.Domain.Heroes.Affiliations.Classifications;
using HeroService.Contracts.Persistence.Heroes;
using HeroService.Contracts.Queries.Heroes.Classification.MythologyTypes;

namespace HeroService.Application.Queries.Heroes.Classification.MythologyTypes;

public sealed class GetMythologyTypeByNameQueryHandler
    : IQueryHandler<GetMythologyTypeByNameQuery, MythologyTypeDto?>
{
  private readonly IMythologyRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetMythologyTypeByNameQueryHandler(
      IMythologyRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<MythologyTypeDto?> Handle(
      GetMythologyTypeByNameQuery query,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByNameAsync(query.Name, cancellationToken);

    if (entity is null)
      return null;

    return _mapper.Map<MythologyType, MythologyTypeDto>(entity);
  }
}
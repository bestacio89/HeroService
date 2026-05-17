using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes.Classes;
using HeroService.Domain.Heroes.Core;

namespace HeroService.Application.Queries.Heroes.Classes;

public sealed class GetHeroClassByIdQueryHandler
    : IQueryHandler<GetHeroClassByIdQuery, HeroClassDto?>
{
  private readonly IEntityRepository<HeroClass, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetHeroClassByIdQueryHandler(
      IEntityRepository<HeroClass, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<HeroClassDto?> Handle(
      GetHeroClassByIdQuery query,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(query.Id, cancellationToken);

    if (entity is null)
      return null;

    return _mapper.Map<HeroClass, HeroClassDto>(entity);
  }
}
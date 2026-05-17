using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mapping.Abstractions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Domain.Heroes.Core;
using HeroService.Contracts.Queries.Heroes.Classes;

namespace HeroService.Application.Queries.Heroes.Classes;

public sealed class GetAllHeroClassesQueryHandler
    : IQueryHandler<GetAllHeroClassesQuery, IReadOnlyCollection<HeroClassDto>>
{
  private readonly IEntityRepository<HeroClass, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetAllHeroClassesQueryHandler(
      IEntityRepository<HeroClass, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyCollection<HeroClassDto>> Handle(
      GetAllHeroClassesQuery query,
      CancellationToken cancellationToken)
  {
    var entities = await _repository.GetAllAsync(cancellationToken);

    return _mapper.Map<IReadOnlyCollection<HeroClass>, IReadOnlyCollection<HeroClassDto>>(entities);
  }
}
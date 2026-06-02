
using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.Queries.Heroes;
using HeroService.Domain.Heroes.Progression;

namespace HeroService.Application.Queries.Heroes;

public sealed class GetHeroProgressionByHeroQueryHandler : IQueryHandler<GetHeroProgressionByHeroQuery, HeroProgressionModifiersDto?>
{
  private readonly IEntityRepository<HeroProgressionModifiers, Guid> _repo;
  private readonly IFranzMapper _mapper;

  public GetHeroProgressionByHeroQueryHandler(IEntityRepository<HeroProgressionModifiers, Guid> repo, IFranzMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<HeroProgressionModifiersDto?> Handle(GetHeroProgressionByHeroQuery request, CancellationToken ct)
  {
    var records = await _repo.GetAllAsync(ct);
    var target = records.FirstOrDefault(p => p.HeroId == request.HeroId);

    return target == null ? null : _mapper.Map<HeroProgressionModifiers,HeroProgressionModifiersDto>(target);
  }
}
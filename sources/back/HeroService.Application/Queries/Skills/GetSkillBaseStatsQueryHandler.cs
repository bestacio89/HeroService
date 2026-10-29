using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Queries.Skills;

/// <summary>
/// Retrieves the base-stat definition associated with a skill.
/// </summary>
public sealed class GetSkillBaseStatsQueryHandler
    : IQueryHandler<GetSkillBaseStatsQuery, SkillBaseStatsDto?>
{
  private readonly ISkillBaseStatsRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetSkillBaseStatsQueryHandler(
      ISkillBaseStatsRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<SkillBaseStatsDto?> Handle(
      GetSkillBaseStatsQuery query,
      CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(query);

    var statsBySkillId = await _repository.GetBySkillIdsAsync(
        [query.SkillId],
        cancellationToken);

    if (!statsBySkillId.TryGetValue(query.SkillId, out var stats))
      return null;

    return _mapper.Map<SkillBaseStats, SkillBaseStatsDto>(stats);
  }
}

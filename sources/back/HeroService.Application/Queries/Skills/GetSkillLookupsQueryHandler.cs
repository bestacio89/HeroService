using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Skills;

namespace HeroService.Application.Queries.Skills;

public sealed class GetSkillLookupsQueryHandler
    : IQueryHandler<GetSkillLookupsQuery, IReadOnlyCollection<SkillLookupDto>>
{
  private readonly ISkillRepository _repository;

  public GetSkillLookupsQueryHandler(ISkillRepository repository)
  {
    _repository = repository;
  }

  public async Task<IReadOnlyCollection<SkillLookupDto>> Handle(
      GetSkillLookupsQuery query,
      CancellationToken cancellationToken)
  {
    ArgumentNullException.ThrowIfNull(query);
    ArgumentNullException.ThrowIfNull(query.SkillIds);

    var ids = query.SkillIds
        .Where(id => id != Guid.Empty)
        .Distinct()
        .ToArray();

    if (ids.Length == 0)
      return Array.Empty<SkillLookupDto>();

    var skills = await _repository.GetLookupsByIdsAsync(ids, cancellationToken);

    return skills
        .Select(skill => new SkillLookupDto(skill.Id, skill.Name))
        .ToArray();
  }
}

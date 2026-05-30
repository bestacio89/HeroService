using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Queries.Skills;

public sealed class GetSkillsByTypeQueryHandler
    : IQueryHandler<GetSkillsByTypeQuery, IReadOnlyCollection<SkillDto>>
{
  private readonly ISkillRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetSkillsByTypeQueryHandler(
      ISkillRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyCollection<SkillDto>> Handle(
      GetSkillsByTypeQuery query,
      CancellationToken cancellationToken)
  {
    var skills = await _repository.GetByTypeAsync(
        query.SkillType,
        cancellationToken);

    return _mapper.Map<
        IReadOnlyCollection<Skill>,
        IReadOnlyCollection<SkillDto>>(skills);
  }
}
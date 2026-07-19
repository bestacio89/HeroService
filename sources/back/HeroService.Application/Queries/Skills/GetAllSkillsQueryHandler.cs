using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Queries.Skills;

public sealed class GetAllSkillsQueryHandler
    : IQueryHandler<GetAllSkillsQuery, IReadOnlyCollection<SkillDto>>
{
  private readonly IEntityRepository<Skill, Guid> _repository;
  private readonly IFranzMapper _mapper;

  public GetAllSkillsQueryHandler(
      IEntityRepository<Skill, Guid> repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<IReadOnlyCollection<SkillDto>> Handle(
      GetAllSkillsQuery query,
      CancellationToken cancellationToken)
  {
    var skills = await _repository.GetAllAsync(
        cancellationToken);

    return _mapper.Map<
        IReadOnlyCollection<Skill>,
        IReadOnlyCollection<SkillDto>>(skills);
  }
}
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Queries.Skills;

public sealed class GetSkillByNameQueryHandler
    : IQueryHandler<GetSkillByNameQuery, SkillDto>
{
  private readonly ISkillRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetSkillByNameQueryHandler(
      ISkillRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<SkillDto> Handle(
      GetSkillByNameQuery query,
      CancellationToken cancellationToken)
  {
    var skill = await _repository.GetByNameWithDetailsAsync(
        query.Name,
        cancellationToken);

    if (skill is null)
      throw new KeyNotFoundException(
          $"Skill '{query.Name}' was not found.");

    return _mapper.Map<Skill, SkillDto>(skill);
  }
}
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Queries.Skills;

public sealed class GetSkillDetailsQueryHandler
    : IQueryHandler<GetSkillDetailsQuery, SkillDto>
{
  private readonly ISkillRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetSkillDetailsQueryHandler(
      ISkillRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<SkillDto> Handle(
      GetSkillDetailsQuery query,
      CancellationToken cancellationToken)
  {
    var skill = await _repository.GetDetailsAsync(
        query.SkillId,
        cancellationToken);

    if (skill is null)
      throw new KeyNotFoundException(
          $"Skill '{query.SkillId}' was not found.");

    return _mapper.Map<Skill, SkillDto>(skill);
  }
}
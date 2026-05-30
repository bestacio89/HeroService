using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Persistence.Skills;
using HeroService.Contracts.Queries.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Queries.Skills;

public sealed class GetSkillLoreQueryHandler
    : IQueryHandler<GetSkillLoreQuery, SkillLoreDto>
{
  private readonly ISkillLoreRepository _repository;
  private readonly IFranzMapper _mapper;

  public GetSkillLoreQueryHandler(
      ISkillLoreRepository repository,
      IFranzMapper mapper)
  {
    _repository = repository;
    _mapper = mapper;
  }

  public async Task<SkillLoreDto> Handle(
      GetSkillLoreQuery query,
      CancellationToken cancellationToken)
  {
    var lore = await _repository.GetBySkillIdAsync(
        query.SkillId,
        cancellationToken);

    if (lore is null)
      throw new KeyNotFoundException(
          $"Skill lore for Skill '{query.SkillId}' was not found.");

    return _mapper.Map<SkillLore, SkillLoreDto>(lore);
  }
}
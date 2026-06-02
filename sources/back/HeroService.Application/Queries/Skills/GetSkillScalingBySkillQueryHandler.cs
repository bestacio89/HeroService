using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Contracts.Queries.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Application.Queries.Skills;

public sealed class GetSkillScalingBySkillQueryHandler : IQueryHandler<GetSkillScalingBySkillQuery, SkillScalingModifierDto?>
{
  private readonly IEntityRepository<SkillScalingModifier, Guid> _repo;
  private readonly IFranzMapper _mapper;

  public GetSkillScalingBySkillQueryHandler(IEntityRepository<SkillScalingModifier, Guid> repo, IFranzMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<SkillScalingModifierDto?> Handle(GetSkillScalingBySkillQuery request, CancellationToken ct)
  {
    var records = await _repo.GetAllAsync(ct);
    var target = records.FirstOrDefault(s => s.SkillId == request.SkillId);

    return target == null ? null : _mapper.Map<SkillScalingModifier,SkillScalingModifierDto>(target);
  }
}
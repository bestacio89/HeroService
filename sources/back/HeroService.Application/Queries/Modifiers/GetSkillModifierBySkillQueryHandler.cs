using Franz.Common.Business.Repositories;
using Franz.Common.Mapping;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Queries.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Application.Queries.Modifiers;

public sealed class GetSkillModifierBySkillQueryHandler
    : IQueryHandler<GetSkillModifierBySkillQuery, SkillModifierDto>
{
  private readonly IGameVersionRepository _gameVersions;
  private readonly IEntityRepository<SkillModifier, Guid> _skillModifiers;
  private readonly IFranzMapper _mapper;

  public GetSkillModifierBySkillQueryHandler(
      IGameVersionRepository gameVersions,
      IEntityRepository<SkillModifier, Guid> skillModifiers,
      IFranzMapper mapper)
  {
    _gameVersions = gameVersions;
    _skillModifiers = skillModifiers;
    _mapper = mapper;
  }

  public async Task<SkillModifierDto> Handle(
      GetSkillModifierBySkillQuery request,
      CancellationToken cancellationToken)
  {
    var activeVersion = await _gameVersions.GetActiveAsync(cancellationToken)
        ?? throw new InvalidOperationException("No active GameVersion found.");

    var modifier = (await _skillModifiers.GetAllAsync(cancellationToken))
        .FirstOrDefault(x =>
            x.GameVersionId == activeVersion.Id &&
            x.SkillId == request.SkillId);

    if (modifier is null)
    {
      throw new InvalidOperationException(
          $"No SkillModifier found for Skill '{request.SkillId}' in active GameVersion '{activeVersion.Id}'.");
    }

    return _mapper.Map<SkillModifier ,SkillModifierDto>(modifier);
  }
}
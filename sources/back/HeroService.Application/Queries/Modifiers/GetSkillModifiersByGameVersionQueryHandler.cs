using Franz.Common.Business.Repositories;
using Franz.Common.Mapping;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Queries.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Application.Queries.Modifiers;

public sealed class GetSkillModifiersByGameVersionQueryHandler
    : IQueryHandler<GetSkillModifiersByGameVersionQuery, IReadOnlyList<SkillModifierDto>>
{
  private readonly IGameVersionRepository _gameVersions;
  private readonly IEntityRepository<SkillModifier, Guid> _skillModifiers;
  private readonly IFranzMapper _mapper;

  public GetSkillModifiersByGameVersionQueryHandler(
      IGameVersionRepository gameVersions,
      IEntityRepository<SkillModifier, Guid> skillModifiers,
      IFranzMapper mapper)
  {
    _gameVersions = gameVersions;
    _skillModifiers = skillModifiers;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<SkillModifierDto>> Handle(
      GetSkillModifiersByGameVersionQuery request,
      CancellationToken cancellationToken)
  {
    var activeVersion = await _gameVersions.GetActiveAsync(cancellationToken)
        ?? throw new InvalidOperationException("No active GameVersion found.");

    var modifiers = (await _skillModifiers.GetAllAsync(cancellationToken))
        .Where(x => x.GameVersionId == activeVersion.Id)
        .ToList();

    return _mapper.Map<IReadOnlyList<SkillModifier>,IReadOnlyList<SkillModifierDto>>(modifiers);
  }
}
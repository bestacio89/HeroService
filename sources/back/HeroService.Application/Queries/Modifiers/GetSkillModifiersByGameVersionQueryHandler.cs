using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Queries.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Application.Queries.Modifiers;

public sealed class GetSkillModifiersByGameVersionQueryHandler
    : IQueryHandler<GetSkillModifiersByGameVersionQuery, IReadOnlyList<SkillModifierDto>>
{
  private readonly IGameVersionRepository _gameVersions;
  private readonly ISkillModifierRepository _skillModifiers;
  private readonly IFranzMapper _mapper;

  public GetSkillModifiersByGameVersionQueryHandler(
      IGameVersionRepository gameVersions,
      ISkillModifierRepository skillModifiers,
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

    var modifiers = await _skillModifiers.GetByGameVersionIdAsync(
        activeVersion.Id,
        cancellationToken);

    return _mapper.Map<
        IReadOnlyList<SkillModifier>,
        IReadOnlyList<SkillModifierDto>>(modifiers);
  }
}

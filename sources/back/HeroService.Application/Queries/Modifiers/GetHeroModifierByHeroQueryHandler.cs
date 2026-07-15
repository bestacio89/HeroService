using Franz.Common.Business.Repositories;
using Franz.Common.Mapping;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Queries.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Application.Queries.Modifiers;

public sealed class GetHeroModifierByHeroQueryHandler
    : IQueryHandler<GetHeroModifierByHeroQuery, HeroModifierDto>
{
  private readonly IGameVersionRepository _gameVersions;
  private readonly IEntityRepository<HeroModifier, Guid> _heroModifiers;
  private readonly IFranzMapper _mapper;

  public GetHeroModifierByHeroQueryHandler(
      IGameVersionRepository gameVersions,
      IEntityRepository<HeroModifier, Guid> heroModifiers,
      IFranzMapper mapper)
  {
    _gameVersions = gameVersions;
    _heroModifiers = heroModifiers;
    _mapper = mapper;
  }

  public async Task<HeroModifierDto> Handle(
      GetHeroModifierByHeroQuery request,
      CancellationToken cancellationToken)
  {
    var activeVersion = await _gameVersions.GetActiveAsync(cancellationToken)
        ?? throw new InvalidOperationException("No active GameVersion found.");

    var modifier = (await _heroModifiers.GetAllAsync(cancellationToken))
        .FirstOrDefault(x =>
            x.GameVersionId == activeVersion.Id &&
            x.HeroId == request.HeroId);

    if (modifier is null)
      throw new InvalidOperationException(
          $"No HeroModifier found for Hero '{request.HeroId}' in active GameVersion '{activeVersion.Id}'.");

    return _mapper.Map< HeroModifier, HeroModifierDto>(modifier);
  }
}
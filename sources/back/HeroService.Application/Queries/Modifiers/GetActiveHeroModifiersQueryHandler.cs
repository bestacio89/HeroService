using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Contracts.Queries.Modifiers;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Application.Queries.Modifiers;

public sealed class GetActiveHeroModifiersQueryHandler
    : IQueryHandler<GetActiveHeroModifiersQuery, IReadOnlyList<HeroModifierDto>>
{
  private readonly IGameVersionRepository _versions;
  private readonly IEntityRepository<HeroModifier, Guid> _modifiers;


  public GetActiveHeroModifiersQueryHandler(
      IGameVersionRepository versions,
      IEntityRepository<HeroModifier, Guid> modifiers)
  {
    _versions = versions;
    _modifiers = modifiers;
  }


  public async Task<IReadOnlyList<HeroModifierDto>> Handle(
      GetActiveHeroModifiersQuery request,
      CancellationToken cancellationToken)
  {
    var activeVersion =
        await _versions.GetActiveAsync(cancellationToken)
        ?? throw new InvalidOperationException(
            "No active GameVersion found.");


    var modifiers =
        await _modifiers.GetAllAsync(cancellationToken);


    return modifiers
        .Where(x =>
            x.GameVersionId == activeVersion.Id)
        .Select(x =>
            new HeroModifierDto(
                x.HeroId,
                x.GameVersionId,

                x.HealthMultiplier,
                x.ManaMultiplier,

                x.AttackDamageMultiplier,
                x.MagicDamageMultiplier,
                x.IgnoreEnemyDefenseAdjustment,

                x.AttackSpeedMultiplier,
                x.CastSpeedMultiplier,

                x.CritChanceMultiplier,
                x.CritDamageMultiplier,

                x.ArmorMultiplier,
                x.MagicResistanceMultiplier,
                x.DamageReductionMultiplier,

                x.ShieldStrengthMultiplier,

                x.MovementSpeedMultiplier,
                x.AttackRangeMultiplier,

                x.CooldownReductionMultiplier,
                x.ResourceRegenerationMultiplier
            ))
        .ToList();
  }
}
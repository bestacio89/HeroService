using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.DTOs.Modifiers;
using HeroService.Contracts.Persistence.GameVersions;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Contracts.Queries.Modifiers;

namespace HeroService.Application.Queries.Modifiers;

public sealed class GetActiveHeroModifiersQueryHandler
    : IQueryHandler<GetActiveHeroModifiersQuery, IReadOnlyList<HeroModifierDto>>
{
  private readonly IGameVersionRepository _versions;
  private readonly IHeroModifierRepository _modifiers;

  public GetActiveHeroModifiersQueryHandler(
      IGameVersionRepository versions,
      IHeroModifierRepository modifiers)
  {
    _versions = versions;
    _modifiers = modifiers;
  }

  public async Task<IReadOnlyList<HeroModifierDto>> Handle(
      GetActiveHeroModifiersQuery request,
      CancellationToken cancellationToken)
  {
    var activeVersion = await _versions.GetActiveAsync(cancellationToken)
        ?? throw new InvalidOperationException("No active GameVersion found.");

    var modifiers = await _modifiers.GetByGameVersionIdAsync(
        activeVersion.Id,
        cancellationToken);

    return modifiers
        .Select(modifier => new HeroModifierDto(
            modifier.HeroId,
            modifier.GameVersionId,
            modifier.HealthMultiplier,
            modifier.ManaMultiplier,
            modifier.AttackDamageMultiplier,
            modifier.MagicDamageMultiplier,
            modifier.IgnoreEnemyDefenseAdjustment,
            modifier.AttackSpeedMultiplier,
            modifier.CastSpeedMultiplier,
            modifier.CritChanceMultiplier,
            modifier.CritDamageMultiplier,
            modifier.ArmorMultiplier,
            modifier.MagicResistanceMultiplier,
            modifier.DamageReductionMultiplier,
            modifier.ShieldStrengthMultiplier,
            modifier.MovementSpeedMultiplier,
            modifier.AttackRangeMultiplier,
            modifier.CooldownReductionMultiplier,
            modifier.ResourceRegenerationMultiplier))
        .ToArray();
  }
}

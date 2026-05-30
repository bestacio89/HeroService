using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;


public sealed class UpdateHeroModifierCommandHandler
    : ICommandHandler<UpdateHeroModifierCommand>
{
  private readonly IEntityRepository<HeroModifier, Guid> _repository;

  public UpdateHeroModifierCommandHandler(IEntityRepository<HeroModifier,Guid> repository)
  {
    _repository = repository;
  }

  public async Task Handle(
      UpdateHeroModifierCommand request,
      CancellationToken cancellationToken)
  {
    var modifier = await _repository.GetByIdAsync(
        request.HeroModifierId,
        cancellationToken);

    if (modifier is null)
      throw new InvalidOperationException("HeroModifier not found.");

    modifier.Define(
        gameVersionId: modifier.GameVersionId,
        heroId: modifier.HeroId,
        healthMultiplier: request.HealthMultiplier,
        manaMultiplier: request.ManaMultiplier,
        attackDamageMultiplier: request.AttackDamageMultiplier,
        abilityPowerMultiplier: request.AbilityPowerMultiplier,
        attackSpeedMultiplier: request.AttackSpeedMultiplier,
        castSpeedMultiplier: request.CastSpeedMultiplier,
        critChanceMultiplier: request.CritChanceMultiplier,
        critDamageMultiplier: request.CritDamageMultiplier,
        armorMultiplier: request.ArmorMultiplier,
        magicResistanceMultiplier: request.MagicResistanceMultiplier,
        damageReductionMultiplier: request.DamageReductionMultiplier,
        shieldStrengthMultiplier: request.ShieldStrengthMultiplier,
        movementSpeedMultiplier: request.MovementSpeedMultiplier,
        attackRangeMultiplier: request.AttackRangeMultiplier,
        cooldownReductionMultiplier: request.CooldownReductionMultiplier,
        resourceRegenerationMultiplier: request.ResourceRegenerationMultiplier,
        createdBy: "system"
    );

    await _repository.UpdateAsync(modifier, cancellationToken);
  }
}
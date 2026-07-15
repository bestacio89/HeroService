using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Application.Commands.Modifiers;

public sealed class CreateHeroModifierCommandHandler
    : ICommandHandler<CreateHeroModifierCommand, Guid>
{
  private readonly IEntityFactory<Guid, HeroModifier> _factory;
  private readonly IEntityRepository<HeroModifier, Guid> _repository;


  public CreateHeroModifierCommandHandler(
      IEntityFactory<Guid, HeroModifier> factory,
      IEntityRepository<HeroModifier, Guid> repository)
  {
    _factory = factory;
    _repository = repository;
  }


  public async Task<Guid> Handle(
      CreateHeroModifierCommand request,
      CancellationToken cancellationToken)
  {
    var modifier = _factory.Create();


    modifier.Define(
        gameVersionId: request.GameVersionId,
        heroId: request.HeroId,

        healthMultiplier: request.HealthMultiplier,
        manaMultiplier: request.ManaMultiplier,

        attackDamageMultiplier: request.AttackDamageMultiplier,
        magicDamageMultiplier: request.AbilityPowerMultiplier,

        ignoreEnemyDefenseAdjustment: request.IgnoreEnemyDefenseMultiplier,

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


    await _repository.AddAsync(
        modifier,
        cancellationToken);


    return modifier.Id;
  }
}
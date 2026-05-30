using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Modfifiers;
using HeroService.Contracts.Commands.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Application.Commands.Modifiers;

public sealed class CreateSkillModifierCommandHandler
    : ICommandHandler<CreateSkillModifierCommand, Guid>
{
  private readonly IEntityFactory<Guid, SkillModifier> _factory;
  private readonly IEntityRepository<SkillModifier, Guid> _repository;

  public CreateSkillModifierCommandHandler(
      IEntityFactory<Guid, SkillModifier> factory,
      IEntityRepository<SkillModifier, Guid> repository)
  {
    _factory = factory;
    _repository = repository;
  }

  public async Task<Guid> Handle(
      CreateSkillModifierCommand request,
      CancellationToken cancellationToken)
  {
    var modifier = _factory.Create();

    modifier.Define(
        gameVersionId: request.GameVersionId,
        skillId: request.SkillId,

        cooldownMultiplier: request.CooldownMultiplier,
        manaCostMultiplier: request.ManaCostMultiplier,

        damageMultiplier: request.DamageMultiplier,
        healingMultiplier: request.HealingMultiplier,
        shieldMultiplier: request.ShieldMultiplier,

        castTimeMultiplier: request.CastTimeMultiplier,
        channelDurationMultiplier: request.ChannelDurationMultiplier,

        crowdControlDurationMultiplier: request.CrowdControlDurationMultiplier,
        rangeMultiplier: request.RangeMultiplier,

        createdBy: "system" // ideally replace with ICurrentUserService
    );

    await _repository.AddAsync(modifier, cancellationToken);

    return modifier.Id;
  }
}
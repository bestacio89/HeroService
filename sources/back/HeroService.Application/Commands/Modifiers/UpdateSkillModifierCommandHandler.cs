using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Handlers;
using HeroService.Contracts.Commands.Modfifiers;
using HeroService.Contracts.Persistence.Modifiers;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Application.Commands.Modifiers;

public sealed class UpdateSkillModifierCommandHandler
    : ICommandHandler<UpdateSkillModifierCommand>
{
  private readonly IEntityRepository<SkillModifier, Guid> _repository;

  public UpdateSkillModifierCommandHandler(IEntityRepository<SkillModifier, Guid> repository)
  {
    _repository = repository;
  }

  public async Task Handle(
      UpdateSkillModifierCommand request,
      CancellationToken cancellationToken)
  {
    var modifier = await _repository.GetByIdAsync(
        request.SkillModifierId,
        cancellationToken);

    if (modifier is null)
      throw new InvalidOperationException("SkillModifier not found.");

    modifier.Define(
        gameVersionId: modifier.GameVersionId,
        skillId: modifier.SkillId,
        cooldownMultiplier: request.CooldownMultiplier,
        manaCostMultiplier: request.ManaCostMultiplier,
        damageMultiplier: request.DamageMultiplier,
        healingMultiplier: request.HealingMultiplier,
        shieldMultiplier: request.ShieldMultiplier,
        castTimeMultiplier: request.CastTimeMultiplier,
        channelDurationMultiplier: request.ChannelDurationMultiplier,
        crowdControlDurationMultiplier: request.CrowdControlDurationMultiplier,
        rangeMultiplier: request.RangeMultiplier,
        createdBy: "system"
    );

    await _repository.UpdateAsync(modifier, cancellationToken);
  }
}

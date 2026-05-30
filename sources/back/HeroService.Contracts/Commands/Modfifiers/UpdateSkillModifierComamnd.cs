using Franz.Common.Mediator.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.Commands.Modfifiers;

public sealed record UpdateSkillModifierCommand(
    Guid SkillModifierId,
    float CooldownMultiplier,
    float ManaCostMultiplier,
    float DamageMultiplier,
    float HealingMultiplier,
    float ShieldMultiplier,
    float CastTimeMultiplier,
    float ChannelDurationMultiplier,
    float CrowdControlDurationMultiplier,
    float RangeMultiplier
) : ICommand;
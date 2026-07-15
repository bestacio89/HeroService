using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Modifiers;

public sealed record SkillModifierDto(
    Guid SkillId,
    Guid GameVersionId,
    float? CooldownMultiplier,
    float? ManaCostMultiplier,
    float? DamageMultiplier,
    float? HealingMultiplier,
    float? ShieldMultiplier,
    float? CastTimeMultiplier,
    float? ChannelDurationMultiplier,
    float? CrowdControlDurationMultiplier,
    float? RangeMultiplier
);
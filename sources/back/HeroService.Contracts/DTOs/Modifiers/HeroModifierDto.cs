using System;
using System.Collections.Generic;
using System.Text;

namespace HeroService.Contracts.DTOs.Modifiers;

public sealed record HeroModifierDto(
    Guid HeroId,
    Guid GameVersionId,
    float? HealthMultiplier,
    float? ManaMultiplier,
    float? AttackDamageMultiplier,
    float? AbilityPowerMultiplier,
    float? AttackSpeedMultiplier,
    float? CastSpeedMultiplier,
    float? CritChanceMultiplier,
    float? CritDamageMultiplier,
    float? ArmorMultiplier,
    float? MagicResistanceMultiplier,
    float? DamageReductionMultiplier,
    float? ShieldStrengthMultiplier,
    float? MovementSpeedMultiplier,
    float? AttackRangeMultiplier,
    float? CooldownReductionMultiplier,
    float? ResourceRegenerationMultiplier
);
using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Modifiers;

public sealed record UpdateHeroModifierCommand(
    Guid HeroModifierId,
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
) : ICommand;
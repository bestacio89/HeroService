using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Modfifiers;

public sealed record CreateSkillModifierCommand(
    Guid SkillId,
    Guid GameVersionId,
    float CooldownMultiplier,
    float ManaCostMultiplier,
    float DamageMultiplier,
    float HealingMultiplier,
    float ShieldMultiplier,
    float CastTimeMultiplier,
    float ChannelDurationMultiplier,
    float CrowdControlDurationMultiplier,
    float RangeMultiplier
) : ICommand<Guid>;
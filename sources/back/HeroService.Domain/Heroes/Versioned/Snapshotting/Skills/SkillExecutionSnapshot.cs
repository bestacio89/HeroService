namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

public sealed record SkillExecutionSnapshot
(
    float? Cooldown,
    float? ManaCost,

    float? Damage,
    float? Healing,
    float? ShieldValue,

    float? CastTime,
    float? ChannelDuration,

    float? CrowdControlDuration,
    float? Range
);
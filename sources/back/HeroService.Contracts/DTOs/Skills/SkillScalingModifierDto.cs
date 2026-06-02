namespace HeroService.Contracts.DTOs.Skills;

public sealed record SkillScalingModifierDto(
    Guid Id,
    Guid SkillId,
    float AttackDamageRatio,
    float AbilityPowerRatio,
    float MaxHealthRatio);
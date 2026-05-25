namespace HeroService.Contracts.DTOs.Heroes;

public sealed record HeroBaseStatsDto(
    float BaseHealth,
    float BaseMana,

    float BaseAttackDamage,
    float BaseAbilityPower,

    float BaseAttackSpeed,
    float BaseCritChance,
    float BaseCritDamageMultiplier,

    float BaseArmor,
    float BaseMagicResistance,
    float BaseDamageReduction,

    float BaseShieldStrengthMultiplier,

    float BaseMovementSpeed,
    float BaseAttackRange,
    float BaseCastSpeed,

    float BaseCooldownReduction,
    float BaseResourceRegeneration
);
namespace HeroService.Contracts.DTOs.Snapshots;

public sealed record HeroStatSnapshotDto(
    float Health,
    float Mana,

    float AttackDamage,
    float AbilityPower,

    float AttackSpeed,
    float CastSpeed,

    float CritChance,
    float CritDamageMultiplier,

    float Armor,
    float MagicResistance,
    float DamageReduction,

    float ShieldStrengthMultiplier,

    float MovementSpeed,
    float AttackRange,

    float CooldownReduction,
    float ResourceRegeneration
);
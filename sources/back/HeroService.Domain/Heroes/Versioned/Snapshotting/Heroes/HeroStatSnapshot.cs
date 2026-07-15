namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;

public sealed record HeroStatSnapshot
(
    float Health,
    float Mana,

    float AttackDamage,
    float AbilityPower,

    float IgnoreEnemyDefense,

    float AttackSpeed,
    float CritChance,
    float CritDamageMultiplier,

    float Armor,
    float MagicResistance,
    float DamageReduction,
    float ShieldStrengthMultiplier,

    float MovementSpeed,
    float AttackRange,
    float CastSpeed,

    float CooldownReduction,
    float ResourceRegeneration
);
namespace HeroService.Contracts.DTOs.Heroes;

public sealed record HeroProgressionModifiersDto(
    Guid Id,
    Guid HeroId,
    float HealthPerLevel,
    float ManaPerLevel,
    float AttackDamagePerLevel,
    float MagicDamagePerLevel,
    float ArmorPerLevel,
    float MagicResistancePerLevel,
    float AttackSpeedPerLevel,
    float CastSpeedPerLevel,
    float ResourceRegenerationPerLevel);
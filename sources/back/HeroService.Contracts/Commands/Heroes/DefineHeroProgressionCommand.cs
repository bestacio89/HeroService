using HeroService.Contracts.DTOs.Heroes;
using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Heroes;

public sealed record DefineHeroProgressionCommand(
    Guid HeroId,
    float HealthPerLevel,
    float ManaPerLevel,
    float AttackDamagePerLevel,
    float AbilityPowerPerLevel,
    float ArmorPerLevel,
    float MagicResistancePerLevel,
    float AttackSpeedPerLevel,
    float CastSpeedPerLevel,
    float ResourceRegenerationPerLevel,
    string CreatedBy) : ICommand<Guid>;
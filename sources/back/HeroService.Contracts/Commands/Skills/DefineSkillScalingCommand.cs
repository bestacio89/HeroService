using HeroService.Contracts.DTOs.Skills;
using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Skills;

public sealed record DefineSkillScalingCommand(
    Guid SkillId,
    float AttackDamageRatio,
    float AbilityPowerRatio,
    float MaxHealthRatio,
    string CreatedBy) : ICommand<Guid>;
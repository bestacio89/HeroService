using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Modifiers;

public sealed class CreateHeroModifierCommand : ICommand<Guid>
{
  public Guid HeroId { get; set; }

  public Guid GameVersionId { get; set; }


  public float? HealthMultiplier { get; set; }

  public float? ManaMultiplier { get; set; }


  public float? AttackDamageMultiplier { get; set; }

  public float? AbilityPowerMultiplier { get; set; }

  public float? IgnoreEnemyDefenseMultiplier { get; set; }


  public float? AttackSpeedMultiplier { get; set; }

  public float? CastSpeedMultiplier { get; set; }


  public float? CritChanceMultiplier { get; set; }

  public float? CritDamageMultiplier { get; set; }


  public float? ArmorMultiplier { get; set; }

  public float? MagicResistanceMultiplier { get; set; }

  public float? DamageReductionMultiplier { get; set; }


  public float? ShieldStrengthMultiplier { get; set; }


  public float? MovementSpeedMultiplier { get; set; }

  public float? AttackRangeMultiplier { get; set; }


  public float? CooldownReductionMultiplier { get; set; }

  public float? ResourceRegenerationMultiplier { get; set; }
}
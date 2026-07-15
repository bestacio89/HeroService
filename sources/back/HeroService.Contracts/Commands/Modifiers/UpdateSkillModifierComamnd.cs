using Franz.Common.Mediator.Messages;

namespace HeroService.Contracts.Commands.Modifiers;

public sealed class UpdateSkillModifierCommand : ICommand
{
  public Guid SkillModifierId { get; set; }


  public float? CooldownMultiplier { get; set; }

  public float? ManaCostMultiplier { get; set; }

  public float? DamageMultiplier { get; set; }

  public float? HealingMultiplier { get; set; }

  public float? ShieldMultiplier { get; set; }

  public float? CastTimeMultiplier { get; set; }

  public float? ChannelDurationMultiplier { get; set; }

  public float? CrowdControlDurationMultiplier { get; set; }

  public float? RangeMultiplier { get; set; }
}
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Contracts.DTOs.Skills;

public sealed class SkillEffectDto
{
  public EffectType EffectType { get; set; }


  // =========================================================
  // STATE MODIFIER SEMANTICS
  // =========================================================

  public BuffType? BuffType { get; set; }

  public DebuffType? DebuffType { get; set; }


  // =========================================================
  // EFFECT VALUES
  // =========================================================

  public float Magnitude { get; set; }

  public float Duration { get; set; }

  public float Radius { get; set; }


  // =========================================================
  // TARGETING / STACKING
  // =========================================================

  public TargetType TargetType { get; set; }

  public StackType StackType { get; set; }

  public int MaxStacks { get; set; }


  // =========================================================
  // EXECUTION FLAGS
  // =========================================================

  public bool IsPeriodic { get; set; }

  public bool IsInstant { get; set; }

  public bool IsChannelled { get; set; }


  // =========================================================
  // SCALING
  // =========================================================

  public float? AttackDamageRatio { get; set; }

  public float? MagicDamageRatio { get; set; }

  public float? MaxHealthRatio { get; set; }
}
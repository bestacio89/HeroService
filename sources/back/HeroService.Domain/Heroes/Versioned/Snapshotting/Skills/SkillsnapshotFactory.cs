using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

public sealed class SkillSnapshotFactory
{
  public SkillSnapshot Create(
      Skill skill,
      SkillBaseStats baseStats,
      SkillModifier? modifier,
      Guid gameVersionId)
  {
    var execution = BuildExecution(baseStats, modifier);
    var effects = BuildEffects(skill.Effects);

    return new SkillSnapshot(
        skill.Id,
        gameVersionId,
        execution,
        effects
    );
  }

  // =========================================
  // EXECUTION SNAPSHOT (NUMERICAL RESOLUTION)
  // =========================================
  private static SkillExecutionSnapshot BuildExecution(
      SkillBaseStats baseStats,
      SkillModifier? modifier)
  {
    float Apply(float value, float? multiplier)
        => value * (multiplier ?? 1f);

    return new SkillExecutionSnapshot(
        Apply(baseStats.BaseCooldown, modifier?.CooldownMultiplier),
        Apply(baseStats.BaseManaCost, modifier?.ManaCostMultiplier),

        Apply(baseStats.BaseDamage, modifier?.DamageMultiplier),
        baseStats.BaseHealing,
        baseStats.BaseShieldValue,

        baseStats.BaseCastTime,
        baseStats.BaseChannelDuration,
        baseStats.BaseRange,

        baseStats.BaseCrowdControlDuration,

        baseStats.AttackDamageRatio,
        baseStats.AbilityPowerRatio,
        baseStats.MaxHealthRatio
    );
  }

  // =========================================
  // EFFECT SNAPSHOT (SEMANTIC RESOLUTION)
  // =========================================
  private static SkillEffectSnapshot BuildEffects(IEnumerable<SkillEffect> effects)
  {
    var list = effects as IList<SkillEffect> ?? effects.ToList();

    return new SkillEffectSnapshot(
        list.Any(e => e.EffectType is EffectType.Damage or EffectType.DamageOverTime),
        list.Any(e => e.EffectType is EffectType.Heal or EffectType.HealOverTime),
        list.Any(e => e.EffectType == EffectType.Shield),
        list.Any(e => e.EffectType == EffectType.CrowdControl),
        list.Any(e => e.EffectType == EffectType.Mobility),
        list.Any(e => e.EffectType == EffectType.Buff),
        list.Any(e => e.EffectType == EffectType.Debuff),
        list.Any(e => e.EffectType == EffectType.Execute)
    );
  }
}
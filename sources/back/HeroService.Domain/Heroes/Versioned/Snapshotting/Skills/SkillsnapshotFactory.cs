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

        baseStats.BaseCrowdControlDuration

   
    );
  }

  // =========================================
  // EFFECT SNAPSHOT (SEMANTIC RESOLUTION)
  // =========================================
  private static SkillEffectSnapshot BuildEffects(IEnumerable<SkillEffect> effects)
  {
    return new SkillEffectSnapshot(
        effects.Any(e => e.EffectType == EffectType.Damage),
        effects.Any(e => e.EffectType == EffectType.DamageOverTime),
        effects.Any(e => e.EffectType == EffectType.Heal),
        effects.Any(e => e.EffectType == EffectType.HealOverTime),
        effects.Any(e => e.EffectType == EffectType.Shield),

        effects.Any(e => e.EffectType == EffectType.CrowdControl),
        effects.Any(e => e.EffectType == EffectType.Buff),
        effects.Any(e => e.EffectType == EffectType.Debuff),
        effects.Any(e => e.EffectType == EffectType.Mobility),
        effects.Any(e => e.EffectType == EffectType.Execute),

        effects.Any(e => e.EffectType == EffectType.Utility),
        effects.Any(e => e.EffectType == EffectType.Vision),
        effects.Any(e => e.EffectType == EffectType.ZoneControl),
        effects.Any(e => e.EffectType == EffectType.Summon),
        effects.Any(e => e.EffectType == EffectType.Transformation)
    );
  }
}


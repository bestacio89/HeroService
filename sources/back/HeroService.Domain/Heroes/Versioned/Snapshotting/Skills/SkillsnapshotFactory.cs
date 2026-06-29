using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

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
  // EXECUTION SNAPSHOT (NULL-SAFE RESOLUTION)
  // =========================================
  private static SkillExecutionSnapshot BuildExecution(
      SkillBaseStats s,
      SkillModifier? m)
  {
    float? Apply(float? value, float? multiplier)
    {
      if (!value.HasValue)
        return null;

      return value.Value * (multiplier ?? 1f);
    }

    return new SkillExecutionSnapshot(
        Apply(s.BaseCooldown, m?.CooldownMultiplier),
        Apply(s.BaseManaCost, m?.ManaCostMultiplier),

        Apply(s.BaseDamage, m?.DamageMultiplier),
        Apply(s.BaseHealing, m?.HealingMultiplier),
        Apply(s.BaseShieldValue, m?.ShieldMultiplier),

        Apply(s.BaseCastTime, m?.CastTimeMultiplier),
        Apply(s.BaseChannelDuration, m?.ChannelDurationMultiplier),

        Apply(s.BaseCrowdControlDuration, m?.CrowdControlDurationMultiplier),
        Apply(s.BaseRange, m?.RangeMultiplier)
    );
  }

  // =========================================
  // EFFECT SNAPSHOT (UNCHANGED LOGIC)
  // =========================================
  private static SkillEffectSnapshot BuildEffects(IEnumerable<SkillEffect> effects)
  {
    var list = effects as IReadOnlyList<SkillEffect> ?? effects.ToList();

    bool Has(EffectType t) => list.Any(e => e.EffectType == t);

    return new SkillEffectSnapshot(
        Has(EffectType.Damage),
        Has(EffectType.DamageOverTime),
        Has(EffectType.Heal),
        Has(EffectType.HealOverTime),
        Has(EffectType.Shield),

        Has(EffectType.CrowdControl),
        Has(EffectType.Buff),
        Has(EffectType.Debuff),
        Has(EffectType.Mobility),
        Has(EffectType.Execute),

        Has(EffectType.Utility),
        Has(EffectType.Vision),
        Has(EffectType.ZoneControl),
        Has(EffectType.Summon),
        Has(EffectType.Transformation)
    );
  }
}
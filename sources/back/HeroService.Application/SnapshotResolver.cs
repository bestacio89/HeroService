using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

namespace HeroService.Application.Heroes.Versioned.Snapshotting;

public sealed class SnapshotResolver
{
  public HeroSnapshot ResolveHero(
      Guid heroId,
      Guid gameVersionId,
      HeroBaseStats baseStats,
      HeroModifier? heroModifier,
      IReadOnlyList<Skill> skills,
      IReadOnlyDictionary<Guid, SkillBaseStats> skillBaseStats,
      IReadOnlyDictionary<Guid, SkillModifier?> skillModifiers)
  {
    var stats = BuildHeroStats(baseStats, heroModifier);

    var skillKit = new HeroSkillKitSnapshot(
        ResolveSkill(GetSkillByIndex(skills, 0), gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(GetSkillByIndex(skills, 1), gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(GetSkillByIndex(skills, 2), gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(GetSkillByIndex(skills, 3), gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(GetSkillByIndex(skills, 4), gameVersionId, skillBaseStats, skillModifiers)
    );

    return new HeroSnapshot(
        heroId,
        gameVersionId,
        stats,
        skillKit
    );
  }

  // =========================================================
  // HERO STATS SNAPSHOT
  // =========================================================
  private static HeroStatSnapshot BuildHeroStats(
      HeroBaseStats baseStats,
      HeroModifier? modifier)
  {
    float Apply(float value, float? multiplier)
    {
      ValidatePositive(value);
      return value * (multiplier ?? 1f);
    }

    return new HeroStatSnapshot(
        Apply(baseStats.BaseHealth, modifier?.HealthMultiplier),
        Apply(baseStats.BaseMana, modifier?.ManaMultiplier),

        Apply(baseStats.BaseAttackDamage, modifier?.AttackDamageMultiplier),
        Apply(baseStats.BaseAbilityPower, modifier?.AbilityPowerMultiplier),

        Apply(baseStats.BaseAttackSpeed, modifier?.AttackSpeedMultiplier),
        Apply(baseStats.BaseCritChance, modifier?.CritChanceMultiplier),
        Apply(baseStats.BaseCritDamageMultiplier, modifier?.CritDamageMultiplier),

        Apply(baseStats.BaseArmor, modifier?.ArmorMultiplier),
        Apply(baseStats.BaseMagicResistance, modifier?.MagicResistanceMultiplier),
        Apply(baseStats.BaseDamageReduction, modifier?.DamageReductionMultiplier),

        Apply(baseStats.BaseMovementSpeed, modifier?.MovementSpeedMultiplier),
        Apply(baseStats.BaseAttackRange, modifier?.AttackRangeMultiplier),

        Apply(baseStats.BaseCastSpeed, modifier?.CastSpeedMultiplier),
        Apply(baseStats.BaseCooldownReduction, modifier?.CooldownReductionMultiplier),

        Apply(baseStats.BaseResourceRegeneration, modifier?.ResourceRegenerationMultiplier)
    );
  }

  // =========================================================
  // SKILL SNAPSHOT
  // =========================================================
  private static SkillSnapshot ResolveSkill(
      Skill skill,
      Guid gameVersionId,
      IReadOnlyDictionary<Guid, SkillBaseStats> baseStatsMap,
      IReadOnlyDictionary<Guid, SkillModifier?> modifiers)
  {
    var baseStats = baseStatsMap.TryGetValue(skill.Id, out var bs)
        ? bs
        : throw new InvalidOperationException($"Missing SkillBaseStats for Skill {skill.Id}");

    modifiers.TryGetValue(skill.Id, out var modifier);

    var execution = BuildExecution(baseStats, modifier);
    var effects = BuildEffects(skill.Effects);

    return new SkillSnapshot(
        skill.Id,
        gameVersionId,
        execution,
        effects
    );
  }

  // =========================================================
  // SKILL EXECUTION SNAPSHOT
  // =========================================================
  private static SkillExecutionSnapshot BuildExecution(
      SkillBaseStats baseStats,
      SkillModifier? modifier)
  {
    float Apply(float value, float? multiplier)
    {
      ValidatePositive(value);
      return value * (multiplier ?? 1f);
    }

    return new SkillExecutionSnapshot(
        Apply(baseStats.BaseCooldown, modifier?.CooldownMultiplier),
        Apply(baseStats.BaseManaCost, modifier?.ManaCostMultiplier),

        Apply(baseStats.BaseDamage, modifier?.DamageMultiplier),

        Apply(baseStats.BaseHealing, modifier?.HealingMultiplier),
        Apply(baseStats.BaseShieldValue, modifier?.ShieldMultiplier),

        Apply(baseStats.BaseCastTime, modifier?.CastTimeMultiplier),
        Apply(baseStats.BaseChannelDuration, modifier?.ChannelDurationMultiplier),

        Apply(baseStats.BaseCrowdControlDuration, modifier?.CrowdControlDurationMultiplier),
        Apply(baseStats.BaseRange, modifier?.RangeMultiplier)
    );
  }

  // =========================================================
  // EFFECT SNAPSHOT
  // =========================================================
      private static SkillEffectSnapshot BuildEffects(IEnumerable<SkillEffect> effects)
  {
    return new SkillEffectSnapshot(
        effects.Any(e => e.EffectType == EffectType.Damage),

        effects.Any(e => e.EffectType == EffectType.DamageOverTime),

        effects.Any(e => e.EffectType == EffectType.Heal),

        effects.Any(e => e.EffectType == EffectType.HealOverTime),

        effects.Any(e => e.EffectType == EffectType.Shield),

        effects.Any(e => e.EffectType == EffectType.CrowdControl),

        effects.Any(e => e.EffectType == EffectType.Mobility),

        effects.Any(e => e.EffectType == EffectType.Buff),

        effects.Any(e => e.EffectType == EffectType.Debuff),

        effects.Any(e => e.EffectType == EffectType.Execute)
    );
  }
  

  // =========================================================
  // UTILITIES
  // =========================================================
  private static Skill GetSkillByIndex(IReadOnlyList<Skill> skills, int index)
  {
    if (skills.Count <= index)
      throw new InvalidOperationException($"Missing skill at index {index}");

    return skills[index];
  }

  private static void ValidatePositive(float value)
  {
    if (value < 0)
      throw new ArgumentOutOfRangeException(nameof(value), "Stat values cannot be negative.");
  }
}
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

namespace HeroService.Application.Heroes.Versioned.Snapshotting;

public sealed class SnapshotResolver
{
  // =========================================================
  // THRESHOLDS — tunable by game design
  // =========================================================

  /// <summary>Minimum damage skills to be considered burst-oriented.</summary>
  private const int BurstDamageThreshold = 3;

  /// <summary>Minimum sustain skills to be considered sustain-oriented.</summary>
  private const int SustainThreshold = 2;

  /// <summary>Minimum CC skills to be considered control-oriented.</summary>
  private const int ControlThreshold = 2;

  /// <summary>Minimum mobility skills to be considered mobility-oriented.</summary>
  private const int MobilityThreshold = 2;


  // =========================================================
  // ENTRY POINT
  // =========================================================

  public HeroSnapshot ResolveHero(
      Guid heroId,
      Guid gameVersionId,
      HeroBaseStats baseStats,
      HeroModifier? heroModifier,
      IReadOnlyList<Skill> skills,
      IReadOnlyDictionary<Guid, SkillBaseStats> skillBaseStats,
      IReadOnlyDictionary<Guid, SkillModifier> skillModifiers)
  {
    var stats = BuildHeroStats(baseStats, heroModifier);

    var skillKit = new HeroSkillKitSnapshot(
        passive: ResolveSkill(GetSkillByIndex(skills, 0), gameVersionId, skillBaseStats, skillModifiers),
        primary: ResolveSkill(GetSkillByIndex(skills, 1), gameVersionId, skillBaseStats, skillModifiers),
        secondary: ResolveSkill(GetSkillByIndex(skills, 2), gameVersionId, skillBaseStats, skillModifiers),
        tertiary: ResolveSkill(GetSkillByIndex(skills, 3), gameVersionId, skillBaseStats, skillModifiers),
        ultimate: ResolveSkill(GetSkillByIndex(skills, 4), gameVersionId, skillBaseStats, skillModifiers)
    );

    var kitProfile = BuildKitProfile(skillKit);

    return new HeroSnapshot(
        heroId,
        gameVersionId,
        stats,
        skillKit,
        kitProfile
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
    Apply(baseStats.BaseShieldStrengthMultiplier, modifier?.ShieldStrengthMultiplier),

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
  // Previously missing: Utility, Vision, ZoneControl, Summon, Transformation
  // =========================================================

  private static SkillEffectSnapshot BuildEffects(IEnumerable<SkillEffect> effects)
  {
    // Materialise once — avoids multiple enumeration
    var list = effects as IReadOnlyList<SkillEffect> ?? effects.ToList();

    bool Has(EffectType t) => list.Any(e => e.EffectType == t);

    return new SkillEffectSnapshot(
        HasDamage: Has(EffectType.Damage),
        HasDamageOverTime: Has(EffectType.DamageOverTime),
        HasHeal: Has(EffectType.Heal),
        HasHealOverTime: Has(EffectType.HealOverTime),
        HasShield: Has(EffectType.Shield),
        HasCrowdControl: Has(EffectType.CrowdControl),
        HasMobility: Has(EffectType.Mobility),
        HasBuff: Has(EffectType.Buff),
        HasDebuff: Has(EffectType.Debuff),
        HasExecute: Has(EffectType.Execute),

        // Previously missing — now tracked
        HasUtility: Has(EffectType.Utility),
        HasVision: Has(EffectType.Vision),
        HasZoneControl: Has(EffectType.ZoneControl),
        HasSummon: Has(EffectType.Summon),
        HasTransformation: Has(EffectType.Transformation)
    );
  }


  // =========================================================
  // KIT PROFILE
  // Computed from the resolved skill kit.
  // Describes the hero's behavioral fingerprint for item affinity
  // and matchmaking composition analysis.
  // =========================================================

  private static HeroKitProfile BuildKitProfile(HeroSkillKitSnapshot kit)
  {
    var skills = kit.AllSkills;

    int damageCount = skills.Count(s => s.Effects.HasDamage);
    int dotCount = skills.Count(s => s.Effects.HasDamageOverTime);
    int ccCount = skills.Count(s => s.Effects.HasCrowdControl);
    int mobilityCount = skills.Count(s => s.Effects.HasMobility);

    int sustainCount = skills.Count(s =>
        s.Effects.HasHeal ||
        s.Effects.HasHealOverTime ||
        s.Effects.HasShield);

    int utilityCount = skills.Count(s =>
        s.Effects.HasUtility ||
        s.Effects.HasVision ||
        s.Effects.HasZoneControl);

    return new HeroKitProfile(
        DamageSkillCount: damageCount,
        CrowdControlSkillCount: ccCount,
        MobilitySkillCount: mobilityCount,
        SustainSkillCount: sustainCount,
        UtilitySkillCount: utilityCount,

        HasSummon: skills.Any(s => s.Effects.HasSummon),
        HasTransformation: skills.Any(s => s.Effects.HasTransformation),
        HasExecute: skills.Any(s => s.Effects.HasExecute),

        // Derived behavioral tags
        // Burst = heavy damage, no sustained DoT pressure
        IsBurstOriented: damageCount >= BurstDamageThreshold && dotCount == 0,
        IsSustainOriented: sustainCount >= SustainThreshold,
        IsControlOriented: ccCount >= ControlThreshold,
        IsMobilityOriented: mobilityCount >= MobilityThreshold
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
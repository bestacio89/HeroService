using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

namespace HeroService.Application.Heroes.Versioned.Snapshotting;

public sealed class SnapshotResolver
{
  private const int BurstDamageThreshold = 3;
  private const int SustainThreshold = 2;
  private const int ControlThreshold = 2;
  private const int MobilityThreshold = 2;

  // =========================================================
  // HERO ENTRY
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
        ResolveSkill(GetSkill(skills, 0), gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(GetSkill(skills, 1), gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(GetSkill(skills, 2), gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(GetSkill(skills, 3), gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(GetSkill(skills, 4), gameVersionId, skillBaseStats, skillModifiers)
    );

    var profile = BuildKitProfile(skillKit);

    return new HeroSnapshot(heroId, gameVersionId, stats, skillKit, profile);
  }

  // =========================================================
  // HERO STATS
  // =========================================================
  private static HeroStatSnapshot BuildHeroStats(
    HeroBaseStats baseStats,
    HeroModifier? modifier)
  {
    float Apply(float value, float? mult)
        => Safe(value) * (mult ?? 1f);

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
      IReadOnlyDictionary<Guid, SkillModifier> modifiers)
  {
    baseStatsMap.TryGetValue(skill.Id, out var baseStats);
    modifiers.TryGetValue(skill.Id, out var modifier);

    var execution = BuildExecution(baseStats, modifier);
    var effects = BuildEffects(skill.Effects);

    return new SkillSnapshot(skill.Id, gameVersionId, execution, effects);
  }

  // =========================================================
  // EXECUTION (NULL-SAFE CORE FIX)
  // =========================================================
  private static SkillExecutionSnapshot BuildExecution(
      SkillBaseStats? baseStats,
      SkillModifier? modifier)
  {
    float Apply(float? value, float? mult)
        => Safe(value) * (mult ?? 1f);

    return new SkillExecutionSnapshot(
        Apply(baseStats?.BaseCooldown, modifier?.CooldownMultiplier),
        Apply(baseStats?.BaseManaCost, modifier?.ManaCostMultiplier),

        Apply(baseStats?.BaseDamage, modifier?.DamageMultiplier),
        Apply(baseStats?.BaseHealing, modifier?.HealingMultiplier),
        Apply(baseStats?.BaseShieldValue, modifier?.ShieldMultiplier),

        Apply(baseStats?.BaseCastTime, modifier?.CastTimeMultiplier),
        Apply(baseStats?.BaseChannelDuration, modifier?.ChannelDurationMultiplier),

        Apply(baseStats?.BaseRange, modifier?.RangeMultiplier),

        Apply(baseStats?.BaseCrowdControlDuration, modifier?.CrowdControlDurationMultiplier)
    );
  }

  // =========================================================
  // EFFECTS
  // =========================================================
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

  // =========================================================
  // KIT PROFILE
  // =========================================================
  private static HeroKitProfile BuildKitProfile(HeroSkillKitSnapshot kit)
  {
    var skills = kit.AllSkills;

    int damage = skills.Count(s => s.Effects.HasDamage);
    int dot = skills.Count(s => s.Effects.HasDamageOverTime);
    int cc = skills.Count(s => s.Effects.HasCrowdControl);
    int mobility = skills.Count(s => s.Effects.HasMobility);

    int sustain = skills.Count(s =>
        s.Effects.HasHeal ||
        s.Effects.HasHealOverTime ||
        s.Effects.HasShield);

    int utility = skills.Count(s =>
        s.Effects.HasUtility ||
        s.Effects.HasVision ||
        s.Effects.HasZoneControl);

    return new HeroKitProfile(
        damage,
        cc,
        mobility,
        sustain,
        utility,

        skills.Any(s => s.Effects.HasSummon),
        skills.Any(s => s.Effects.HasTransformation),
        skills.Any(s => s.Effects.HasExecute),

        damage >= BurstDamageThreshold && dot == 0,
        sustain >= SustainThreshold,
        cc >= ControlThreshold,
        mobility >= MobilityThreshold
    );
  }

  // =========================================================
  // SAFE ACCESS
  // =========================================================
  private static Skill GetSkill(IReadOnlyList<Skill> skills, int index)
  {
    if (skills.Count <= index)
      throw new InvalidOperationException($"Missing skill at index {index}");

    return skills[index];
  }

  private static float Safe(float? value)
        => value ?? 0f;
}
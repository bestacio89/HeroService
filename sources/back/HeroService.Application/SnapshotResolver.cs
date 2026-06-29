using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Heroes;
using HeroService.Domain.Heroes.Versioned.Snapshotting.Skills;

namespace HeroService.Application.Snapshotting;

public sealed class SnapshotResolver
{
  private const int BurstDamageThreshold = 3;
  private const int SustainThreshold = 2;
  private const int ControlThreshold = 2;
  private const int MobilityThreshold = 2;

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

    var kit = new HeroSkillKitSnapshot(
        ResolveSkill(skills[0], gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(skills[1], gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(skills[2], gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(skills[3], gameVersionId, skillBaseStats, skillModifiers),
        ResolveSkill(skills[4], gameVersionId, skillBaseStats, skillModifiers)
    );

    return new HeroSnapshot(heroId, gameVersionId, stats, kit, BuildKitProfile(kit));
  }

  // =========================================================
  // HERO
  // =========================================================

  private static HeroStatSnapshot BuildHeroStats(HeroBaseStats baseStats, HeroModifier? mod)
  {
    float Apply(float v, float? m) => v * (m ?? 1f);

    return new HeroStatSnapshot(
      Apply(baseStats.BaseHealth, mod?.HealthMultiplier),
      Apply(baseStats.BaseMana, mod?.ManaMultiplier),

      Apply(baseStats.BaseAttackDamage, mod?.AttackDamageMultiplier),
      Apply(baseStats.BaseAbilityPower, mod?.AbilityPowerMultiplier),

      Apply(baseStats.BaseAttackSpeed, mod?.AttackSpeedMultiplier),
      Apply(baseStats.BaseCritChance, mod?.CritChanceMultiplier),
      Apply(baseStats.BaseCritDamageMultiplier, mod?.CritDamageMultiplier),

      Apply(baseStats.BaseArmor, mod?.ArmorMultiplier),
      Apply(baseStats.BaseMagicResistance, mod?.MagicResistanceMultiplier),
      Apply(baseStats.BaseDamageReduction, mod?.DamageReductionMultiplier),
      Apply(baseStats.BaseShieldStrengthMultiplier, mod?.ShieldStrengthMultiplier),

      Apply(baseStats.BaseMovementSpeed, mod?.MovementSpeedMultiplier),
      Apply(baseStats.BaseAttackRange, mod?.AttackRangeMultiplier),
      Apply(baseStats.BaseCastSpeed, mod?.CastSpeedMultiplier),

      Apply(baseStats.BaseCooldownReduction, mod?.CooldownReductionMultiplier),
      Apply(baseStats.BaseResourceRegeneration, mod?.ResourceRegenerationMultiplier)
    );
  }

  // =========================================================
  // SKILL
  // =========================================================

  private static SkillSnapshot ResolveSkill(
      Skill skill,
      Guid gameVersionId,
      IReadOnlyDictionary<Guid, SkillBaseStats> baseStatsMap,
      IReadOnlyDictionary<Guid, SkillModifier?> modifiers)
  {
    if (!baseStatsMap.TryGetValue(skill.Id, out var baseStats))
      throw new InvalidOperationException($"Missing SkillBaseStats for {skill.Id}");

    modifiers.TryGetValue(skill.Id, out var mod);

    return new SkillSnapshot(
      skill.Id,
      gameVersionId,
      BuildExecution(baseStats, mod),
      BuildEffects(skill.Effects)
    );
  }

  private static SkillExecutionSnapshot BuildExecution(SkillBaseStats s, SkillModifier? m)
  {
    float? Apply(float? v, float? mult)
    {
      if (!v.HasValue)
        return null;

      return v.Value * (mult ?? 1f);
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

  // =========================================================
  // EFFECTS (unchanged logic, just stable)
  // =========================================================

  private static SkillEffectSnapshot BuildEffects(IEnumerable<SkillEffect> effects)
  {
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

      HasUtility: Has(EffectType.Utility),
      HasVision: Has(EffectType.Vision),
      HasZoneControl: Has(EffectType.ZoneControl),
      HasSummon: Has(EffectType.Summon),
      HasTransformation: Has(EffectType.Transformation)
    );
  }

  // =========================================================
  // KIT PROFILE (unchanged)
  // =========================================================

  private static HeroKitProfile BuildKitProfile(HeroSkillKitSnapshot kit)
  {
    var skills = kit.AllSkills;

    int damage = skills.Count(s => s.Effects.HasDamage);
    int cc = skills.Count(s => s.Effects.HasCrowdControl);
    int mob = skills.Count(s => s.Effects.HasMobility);

    int sustain = skills.Count(s =>
      s.Effects.HasHeal || s.Effects.HasHealOverTime || s.Effects.HasShield);

    int utility = skills.Count(s =>
      s.Effects.HasUtility || s.Effects.HasVision || s.Effects.HasZoneControl);

    return new HeroKitProfile(
      damage,
      cc,
      mob,
      sustain,
      utility,
      skills.Any(s => s.Effects.HasSummon),
      skills.Any(s => s.Effects.HasTransformation),
      skills.Any(s => s.Effects.HasExecute),
      damage >= BurstDamageThreshold && !skills.Any(s => s.Effects.HasDamageOverTime),
      sustain >= SustainThreshold,
      cc >= ControlThreshold,
      mob >= MobilityThreshold
    );
  }
}
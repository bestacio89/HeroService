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


  public HeroSnapshot ResolveHero(
      Guid heroId,
      string heroName,
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

    return new HeroSnapshot(
        heroId,
        heroName,
        gameVersionId,
        stats,
        kit,
        BuildKitProfile(kit));
  }


  private static HeroStatSnapshot BuildHeroStats(
      HeroBaseStats baseStats,
      HeroModifier? mod)
  {
    float Apply(float v, float? multiplier)
        => v * (multiplier ?? 1f);

    return new HeroStatSnapshot(
        Apply(baseStats.BaseHealth, mod?.HealthMultiplier),
        Apply(baseStats.BaseMana, mod?.ManaMultiplier),
        Apply(baseStats.BaseAttackDamage, mod?.AttackDamageMultiplier),
        Apply(baseStats.BaseMagicDamage, mod?.MagicDamageMultiplier),
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


  private static SkillSnapshot ResolveSkill(
      Skill skill,
      Guid gameVersionId,
      IReadOnlyDictionary<Guid, SkillBaseStats> skillBaseStats,
      IReadOnlyDictionary<Guid, SkillModifier> skillModifiers)
  {
    skillBaseStats.TryGetValue(skill.Id, out var baseStats);
    skillModifiers.TryGetValue(skill.Id, out var modifier);

    return new SkillSnapshot(
        skill.Id,
        skill.Name,
        gameVersionId,
        BuildExecution(baseStats, modifier),
        BuildEffects(skill.Effects),
        BuildEffectExecutions(skill, baseStats, modifier)
    );
  }


  private static SkillExecutionSnapshot BuildExecution(
      SkillBaseStats? stats,
      SkillModifier? modifier)
  {
    float? Apply(float? value, float? multiplier)
        => value.HasValue
            ? value.Value * (multiplier ?? 1f)
            : null;

    return new SkillExecutionSnapshot(
        Apply(stats?.BaseCooldown, modifier?.CooldownMultiplier),
        Apply(stats?.BaseManaCost, modifier?.ManaCostMultiplier),

        Apply(stats?.BaseDamage, modifier?.DamageMultiplier),
        Apply(stats?.BaseHealing, modifier?.HealingMultiplier),
        Apply(stats?.BaseShieldValue, modifier?.ShieldMultiplier),

        Apply(stats?.BaseCastTime, modifier?.CastTimeMultiplier),
        Apply(stats?.BaseChannelDuration, modifier?.ChannelDurationMultiplier),

        Apply(stats?.BaseCrowdControlDuration,
            modifier?.CrowdControlDurationMultiplier),

        Apply(stats?.BaseRange,
            modifier?.RangeMultiplier)
    );
  }


  private static SkillEffectSnapshot BuildEffects(
      IEnumerable<SkillEffect> effects)
  {
    var list = effects as IReadOnlyList<SkillEffect>
               ?? effects.ToList();


    bool Has(EffectType type)
        => list.Any(e => e.EffectType == type);


    var buffTypes = list
        .Where(e => e.EffectType == EffectType.Buff)
        .Where(e => e.BuffType.HasValue)
        .Select(e => e.BuffType!.Value)
        .ToHashSet();


    var debuffTypes = list
        .Where(e => e.EffectType == EffectType.Debuff)
        .Where(e => e.DebuffType.HasValue)
        .Select(e => e.DebuffType!.Value)
        .ToHashSet();


    return new SkillEffectSnapshot(

        // =====================================================
        // DIRECT COMBAT OUTPUT
        // =====================================================

        Has(EffectType.Damage),
        Has(EffectType.DamageOverTime),

        Has(EffectType.Heal),
        Has(EffectType.HealOverTime),

        Has(EffectType.Shield),


        // =====================================================
        // CONTROL SYSTEM
        // =====================================================

        Has(EffectType.Slow),
        Has(EffectType.Root),
        Has(EffectType.Stun),
        Has(EffectType.Silence),
        Has(EffectType.Disarm),
        Has(EffectType.Blind),

        Has(EffectType.Fear),
        Has(EffectType.Charm),
        Has(EffectType.Taunt),
        Has(EffectType.Confuse),
        Has(EffectType.Sleep),

        Has(EffectType.Knockback),
        Has(EffectType.KnockUp),
        Has(EffectType.Pull),

        Has(EffectType.Freeze),
        Has(EffectType.Petrify),


        // =====================================================
        // STATE MODIFIERS
        // =====================================================

        Has(EffectType.Buff),
        Has(EffectType.Debuff),


        // =====================================================
        // POSITIONING
        // =====================================================

        Has(EffectType.Mobility),


        // =====================================================
        // EXECUTION
        // =====================================================

        Has(EffectType.Execute),


        // =====================================================
        // UTILITY
        // =====================================================

        Has(EffectType.Utility),
        Has(EffectType.Vision),
        Has(EffectType.ZoneControl),
        Has(EffectType.Summon),
        Has(EffectType.Transformation),


        // =====================================================
        // STATE MODIFIER SEMANTICS
        // =====================================================

        buffTypes,
        debuffTypes
    );
  }


  private static IReadOnlyList<EffectExecutionSnapshot> BuildEffectExecutions(
      Skill skill,
      SkillBaseStats? baseStats,
      SkillModifier? modifier)
  {
    var results = new List<EffectExecutionSnapshot>();

    foreach (var effect in skill.Effects)
    {
      results.Add(
          ResolveEffect(skill, effect, baseStats, modifier));
    }

    return results;
  }


  private static EffectExecutionSnapshot ResolveEffect(
      Skill skill,
      SkillEffect effect,
      SkillBaseStats? baseStats,
      SkillModifier? modifier)
  {
    float Apply(float value, float? multiplier)
        => value * (multiplier ?? 1f);


    var finalMagnitude =
        Apply(effect.Magnitude,
            modifier?.DamageMultiplier);


    var finalDuration =
        Apply(effect.Duration,
            modifier?.CrowdControlDurationMultiplier);


    var finalRadius =
        Apply(effect.Radius,
            modifier?.RangeMultiplier);


    var stacks =
        effect.StackType == StackType.None
            ? 1
            : effect.MaxStacks;


    float tickInterval =
        effect.IsPeriodic
            ? 0.25f
            : 0f;


    float channelDuration =
        effect.IsChannelled
            ? effect.Duration
            : 0f;


    return new EffectExecutionSnapshot(

        effect.EffectType,

        effect.BuffType,
        effect.DebuffType,

        skill.Id,
        Guid.Empty,
        Array.Empty<Guid>(),

        finalMagnitude,
        finalDuration,
        finalRadius,

        stacks,

        effect.IsInstant,
        effect.IsPeriodic,
        effect.IsChannelled,

        tickInterval,
        channelDuration,

        effect.TargetType
    );
  }


  private static HeroKitProfile BuildKitProfile(
      HeroSkillKitSnapshot kit)
  {
    var skills = kit.AllSkills;


    int damageCount =
        skills.Count(x => x.Effects.HasDamage);


    int crowdControlCount =
        skills.Count(x => x.Effects.HasAnyCrowdControl);


    int mobilityCount =
        skills.Count(x => x.Effects.HasMobility);


    int sustainCount =
        skills.Count(x =>
            x.Effects.HasHeal ||
            x.Effects.HasHealOverTime ||
            x.Effects.HasShield);


    int utilityCount =
        skills.Count(x =>
            x.Effects.HasUtility ||
            x.Effects.HasVision ||
            x.Effects.HasZoneControl);


    return new HeroKitProfile(
        damageCount,
        crowdControlCount,
        mobilityCount,
        sustainCount,
        utilityCount,

        skills.Any(x => x.Effects.HasSummon),
        skills.Any(x => x.Effects.HasTransformation),
        skills.Any(x => x.Effects.HasExecute),

        damageCount >= BurstDamageThreshold &&
        !skills.Any(x => x.Effects.HasDamageOverTime),

        sustainCount >= SustainThreshold,
        crowdControlCount >= ControlThreshold,
        mobilityCount >= MobilityThreshold
    );
  }
}
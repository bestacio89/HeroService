using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion;
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
      GameVersion activeVersion,
      HeroBaseStats baseStats,
      HeroModifier heroModifier,
      IReadOnlyList<Skill> skills,
      IReadOnlyDictionary<Guid, SkillBaseStats> skillBaseStats,
      IReadOnlyDictionary<Guid, SkillModifier> skillModifiers)
  {
    ValidateSnapshotContext(
        activeVersion,
        heroModifier,
        skills,
        skillBaseStats,
        skillModifiers);


    var stats =
        BuildHeroStats(
            baseStats,
            heroModifier);


    var kit =
        new HeroSkillKitSnapshot(
            ResolveSkill(
                skills[0],
                activeVersion.Id,
                skillBaseStats,
                skillModifiers),

            ResolveSkill(
                skills[1],
                activeVersion.Id,
                skillBaseStats,
                skillModifiers),

            ResolveSkill(
                skills[2],
                activeVersion.Id,
                skillBaseStats,
                skillModifiers),

            ResolveSkill(
                skills[3],
                activeVersion.Id,
                skillBaseStats,
                skillModifiers),

            ResolveSkill(
                skills[4],
                activeVersion.Id,
                skillBaseStats,
                skillModifiers)
        );


    return new HeroSnapshot(
        heroId,
        heroName,
        activeVersion.Id,
        stats,
        kit,
        BuildKitProfile(kit));
  }


  private static void ValidateSnapshotContext(
      GameVersion activeVersion,
      HeroModifier heroModifier,
      IReadOnlyList<Skill> skills,
      IReadOnlyDictionary<Guid, SkillBaseStats> skillBaseStats,
      IReadOnlyDictionary<Guid, SkillModifier> skillModifiers)
  {
    if (!activeVersion.IsActive)
    {
      throw new InvalidOperationException(
          $"Cannot resolve snapshot. Game version '{activeVersion.Id}' is not active.");
    }


    if (heroModifier.GameVersionId != activeVersion.Id)
    {
      throw new InvalidOperationException(
          $"Hero modifier does not belong to active version '{activeVersion.Id}'.");
    }


    foreach (var skill in skills)
    {
      if (!skillBaseStats.ContainsKey(skill.Id))
      {
        throw new InvalidOperationException(
            $"Skill '{skill.Name}' has no base stats.");
      }


      if (!skillModifiers.ContainsKey(skill.Id))
      {
        throw new InvalidOperationException(
            $"Skill '{skill.Name}' has no modifier for active version '{activeVersion.Id}'.");
      }
    }
  }


  private static HeroStatSnapshot BuildHeroStats(
      HeroBaseStats baseStats,
      HeroModifier modifier)
  {
    static float Apply(
        float value,
        float? multiplier)
        => value * (multiplier ?? 1f);


    return new HeroStatSnapshot(
        Apply(baseStats.BaseHealth,
            modifier.HealthMultiplier),

        Apply(baseStats.BaseMana,
            modifier.ManaMultiplier),

        Apply(baseStats.BaseAttackDamage,
            modifier.AttackDamageMultiplier),

        Apply(baseStats.BaseMagicDamage,
            modifier.MagicDamageMultiplier),

        Apply(baseStats.BaseAttackSpeed,
            modifier.AttackSpeedMultiplier),

        Apply(baseStats.BaseCritChance,
            modifier.CritChanceMultiplier),

        Apply(baseStats.BaseCritDamageMultiplier,
            modifier.CritDamageMultiplier),

        Apply(baseStats.BaseArmor,
            modifier.ArmorMultiplier),

        Apply(baseStats.BaseMagicResistance,
            modifier.MagicResistanceMultiplier),

        Apply(baseStats.BaseDamageReduction,
            modifier.DamageReductionMultiplier),

        Apply(baseStats.BaseShieldStrengthMultiplier,
            modifier.ShieldStrengthMultiplier),

        Apply(baseStats.BaseMovementSpeed,
            modifier.MovementSpeedMultiplier),

        Apply(baseStats.BaseAttackRange,
            modifier.AttackRangeMultiplier),

        Apply(baseStats.BaseCastSpeed,
            modifier.CastSpeedMultiplier),

        Apply(baseStats.BaseCooldownReduction,
            modifier.CooldownReductionMultiplier),

        Apply(baseStats.BaseResourceRegeneration,
            modifier.ResourceRegenerationMultiplier)
    );
  }


  private static SkillSnapshot ResolveSkill(
      Skill skill,
      Guid versionId,
      IReadOnlyDictionary<Guid, SkillBaseStats> skillBaseStats,
      IReadOnlyDictionary<Guid, SkillModifier> skillModifiers)
  {
    if (!skillBaseStats.TryGetValue(
            skill.Id,
            out var stats))
    {
      throw new InvalidOperationException(
          $"Missing base stats for skill '{skill.Name}'.");
    }


    if (!skillModifiers.TryGetValue(
            skill.Id,
            out var modifier))
    {
      throw new InvalidOperationException(
          $"Missing modifier for skill '{skill.Name}' in version '{versionId}'.");
    }


    return new SkillSnapshot(
        skill.Id,
        skill.Name,
        versionId,
        BuildExecution(stats, modifier),
        BuildEffects(skill.Effects),
        BuildEffectExecutions(
            skill,
            modifier));
  }


  private static SkillExecutionSnapshot BuildExecution(
      SkillBaseStats stats,
      SkillModifier modifier)
  {
    static float? Apply(
        float? value,
        float? multiplier)
        => value.HasValue
            ? value.Value * (multiplier ?? 1f)
            : null;


    return new SkillExecutionSnapshot(
        Apply(
            stats.BaseCooldown,
            modifier.CooldownMultiplier),

        Apply(
            stats.BaseManaCost,
            modifier.ManaCostMultiplier),

        Apply(
            stats.BaseDamage,
            modifier.DamageMultiplier),

        Apply(
            stats.BaseHealing,
            modifier.HealingMultiplier),

        Apply(
            stats.BaseShieldValue,
            modifier.ShieldMultiplier),

        Apply(
            stats.BaseCastTime,
            modifier.CastTimeMultiplier),

        Apply(
            stats.BaseChannelDuration,
            modifier.ChannelDurationMultiplier),

        Apply(
            stats.BaseCrowdControlDuration,
            modifier.CrowdControlDurationMultiplier),

        Apply(
            stats.BaseRange,
            modifier.RangeMultiplier)
    );
  }


  private static SkillEffectSnapshot BuildEffects(
      IEnumerable<SkillEffect> effects)
  {
    var list = effects.ToList();


    bool Has(EffectType type)
        => list.Any(x => x.EffectType == type);


    var buffTypes =
        list
            .Where(x =>
                x.EffectType == EffectType.Buff &&
                x.BuffType.HasValue)
            .Select(x => x.BuffType!.Value)
            .ToHashSet();


    var debuffTypes =
        list
            .Where(x =>
                x.EffectType == EffectType.Debuff &&
                x.DebuffType.HasValue)
            .Select(x => x.DebuffType!.Value)
            .ToHashSet();


    return new SkillEffectSnapshot(
        Has(EffectType.Damage),
        Has(EffectType.DamageOverTime),

        Has(EffectType.Heal),
        Has(EffectType.HealOverTime),

        Has(EffectType.Shield),

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

        Has(EffectType.Buff),
        Has(EffectType.Debuff),

        Has(EffectType.Mobility),

        Has(EffectType.Execute),

        Has(EffectType.Utility),
        Has(EffectType.Vision),
        Has(EffectType.ZoneControl),
        Has(EffectType.Summon),
        Has(EffectType.Transformation),

        buffTypes,
        debuffTypes
    );
  }


  private static IReadOnlyList<EffectExecutionSnapshot> BuildEffectExecutions(
      Skill skill,
      SkillModifier modifier)
  {
    return skill.Effects
        .Select(effect =>
            ResolveEffect(
                skill,
                effect,
                modifier))
        .ToList();
  }


  private static EffectExecutionSnapshot ResolveEffect(
      Skill skill,
      SkillEffect effect,
      SkillModifier modifier)
  {
    static float Apply(
        float value,
        float? multiplier)
        => value * (multiplier ?? 1f);


    return new EffectExecutionSnapshot(
        effect.EffectType,

        effect.BuffType,
        effect.DebuffType,

        skill.Id,
        Guid.Empty,
        Array.Empty<Guid>(),

        Apply(
            effect.Magnitude,
            modifier.DamageMultiplier),

        Apply(
            effect.Duration,
            modifier.CrowdControlDurationMultiplier),

        Apply(
            effect.Radius,
            modifier.RangeMultiplier),

        effect.StackType == StackType.None
            ? 1
            : effect.MaxStacks,

        effect.IsInstant,
        effect.IsPeriodic,
        effect.IsChannelled,

        effect.IsPeriodic
            ? 0.25f
            : 0f,

        effect.IsChannelled
            ? effect.Duration
            : 0f,

        effect.TargetType
    );
  }


  private static HeroKitProfile BuildKitProfile(
      HeroSkillKitSnapshot kit)
  {
    var skills = kit.AllSkills;


    var damageCount =
        skills.Count(x => x.Effects.HasDamage);


    var crowdControlCount =
        skills.Count(x => x.Effects.HasAnyCrowdControl);


    var mobilityCount =
        skills.Count(x => x.Effects.HasMobility);


    var sustainCount =
        skills.Count(x =>
            x.Effects.HasHeal ||
            x.Effects.HasHealOverTime ||
            x.Effects.HasShield);


    var utilityCount =
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
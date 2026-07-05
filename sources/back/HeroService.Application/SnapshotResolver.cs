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

  private static SkillSnapshot ResolveSkill(
      Skill s,
      Guid v,
      IReadOnlyDictionary<Guid, SkillBaseStats> b,
      IReadOnlyDictionary<Guid, SkillModifier> m)
  {
    b.TryGetValue(s.Id, out var bs);
    m.TryGetValue(s.Id, out var ms);

    return new SkillSnapshot(
        s.Id,
        s.Name,
        v,
        BuildExecution(bs, ms),
        BuildEffects(s.Effects),
        BuildEffectExecutions(s, bs, ms)   // ← NEW LAYER
    );
  }

  private static SkillExecutionSnapshot BuildExecution(SkillBaseStats? s, SkillModifier? m)
  {
    float? Apply(float? v, float? mult)
        => v.HasValue ? v.Value * (mult ?? 1f) : null;

    return new SkillExecutionSnapshot(
        Apply(s?.BaseCooldown, m?.CooldownMultiplier),
        Apply(s?.BaseManaCost, m?.ManaCostMultiplier),

        // NOTE: kept for now for compatibility, but semantically belongs to effects layer
        Apply(s?.BaseDamage, m?.DamageMultiplier),
        Apply(s?.BaseHealing, m?.HealingMultiplier),
        Apply(s?.BaseShieldValue, m?.ShieldMultiplier),

        Apply(s?.BaseCastTime, m?.CastTimeMultiplier),
        Apply(s?.BaseChannelDuration, m?.ChannelDurationMultiplier),

        // ⚠️ These two are conceptually questionable long-term
        Apply(s?.BaseCrowdControlDuration, m?.CrowdControlDurationMultiplier),
        Apply(s?.BaseRange, m?.RangeMultiplier)
    );
  }

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
        Has(EffectType.Transformation)
    );
  }

  // =========================================================
  // NEW: EFFECT EXECUTION RESOLUTION LAYER
  // =========================================================

  private static IReadOnlyList<EffectExecutionSnapshot> BuildEffectExecutions(
      Skill skill,
      SkillBaseStats? baseStats,
      SkillModifier? modifier)
  {
    var results = new List<EffectExecutionSnapshot>();

    foreach (var effect in skill.Effects)
    {
      results.Add(ResolveEffect(skill, effect, baseStats, modifier));
    }

    return results;
  }

  private static EffectExecutionSnapshot ResolveEffect(
      Skill skill,
      SkillEffect effect,
      SkillBaseStats? baseStats,
      SkillModifier? modifier)
  {
    float Apply(float v, float? m) => v * (m ?? 1f);

    var finalMagnitude =
        Apply(effect.Magnitude, modifier?.DamageMultiplier);

    var finalDuration =
        Apply(effect.Duration, modifier?.CrowdControlDurationMultiplier);

    var finalRadius =
        Apply(effect.Radius, modifier?.RangeMultiplier);

    var stacks =
        effect.StackType == StackType.None ? 1 : effect.MaxStacks;

    float tickInterval = effect.IsPeriodic ? 0.25f : 0f;
    float channelDuration = effect.IsChannelled ? effect.Duration : 0f;

    return new EffectExecutionSnapshot(
        effect.EffectType,
        skill.Id,
        Guid.Empty, // caster injected at runtime
        Array.Empty<Guid>(), // targets resolved during combat

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

  // =========================================================
  // KIT PROFILE (unchanged logic, now richer data source)
  // =========================================================

  private static HeroKitProfile BuildKitProfile(HeroSkillKitSnapshot kit)
  {
    var s = kit.AllSkills;

    int d = s.Count(x => x.Effects.HasDamage);
    int cc = s.Count(x => x.Effects.HasAnyCrowdControl);
    int mob = s.Count(x => x.Effects.HasMobility);

    int sus = s.Count(x =>
        x.Effects.HasHeal ||
        x.Effects.HasHealOverTime ||
        x.Effects.HasShield);

    int uti = s.Count(x =>
        x.Effects.HasUtility ||
        x.Effects.HasVision ||
        x.Effects.HasZoneControl);

    return new HeroKitProfile(
        d,
        cc,
        mob,
        sus,
        uti,
        s.Any(x => x.Effects.HasSummon),
        s.Any(x => x.Effects.HasTransformation),
        s.Any(x => x.Effects.HasExecute),
        d >= BurstDamageThreshold && !s.Any(x => x.Effects.HasDamageOverTime),
        sus >= SustainThreshold,
        cc >= ControlThreshold,
        mob >= MobilityThreshold
    );
  }
}
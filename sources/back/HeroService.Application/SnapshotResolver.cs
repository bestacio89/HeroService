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

  private static SkillSnapshot ResolveSkill(Skill s, Guid v, IReadOnlyDictionary<Guid, SkillBaseStats> b, IReadOnlyDictionary<Guid, SkillModifier> m)
  {
    b.TryGetValue(s.Id, out var bs);
    m.TryGetValue(s.Id, out var ms);
    return new SkillSnapshot(s.Id, v, BuildExecution(bs, ms), BuildEffects(s.Effects));
  }

  private static SkillExecutionSnapshot BuildExecution(SkillBaseStats? s, SkillModifier? m)
  {
    float? Apply(float? v, float? mult) => v.HasValue ? v.Value * (mult ?? 1f) : null;
    return new SkillExecutionSnapshot(
        Apply(s?.BaseCooldown, m?.CooldownMultiplier),
        Apply(s?.BaseManaCost, m?.ManaCostMultiplier),
        Apply(s?.BaseDamage, m?.DamageMultiplier),
        Apply(s?.BaseHealing, m?.HealingMultiplier),
        Apply(s?.BaseShieldValue, m?.ShieldMultiplier),
        Apply(s?.BaseCastTime, m?.CastTimeMultiplier),
        Apply(s?.BaseChannelDuration, m?.ChannelDurationMultiplier),
        Apply(s?.BaseRange, m?.RangeMultiplier),
        Apply(s?.BaseCrowdControlDuration, m?.CrowdControlDurationMultiplier)
    );
  }

  private static SkillEffectSnapshot BuildEffects(IEnumerable<SkillEffect> effects)
  {
    var list = effects as IReadOnlyList<SkillEffect> ?? effects.ToList();
    bool Has(EffectType t) => list.Any(e => e.EffectType == t);
    return new SkillEffectSnapshot(
        Has(EffectType.Damage), Has(EffectType.DamageOverTime), Has(EffectType.Heal),
        Has(EffectType.HealOverTime), Has(EffectType.Shield), Has(EffectType.CrowdControl),
        Has(EffectType.Buff), Has(EffectType.Debuff), Has(EffectType.Mobility),
        Has(EffectType.Execute), Has(EffectType.Utility), Has(EffectType.Vision),
        Has(EffectType.ZoneControl), Has(EffectType.Summon), Has(EffectType.Transformation)
    );
  }

  private static HeroKitProfile BuildKitProfile(HeroSkillKitSnapshot kit)
  {
    var s = kit.AllSkills;
    int d = s.Count(x => x.Effects.HasDamage), cc = s.Count(x => x.Effects.HasCrowdControl), mob = s.Count(x => x.Effects.HasMobility);
    int sus = s.Count(x => x.Effects.HasHeal || x.Effects.HasHealOverTime || x.Effects.HasShield);
    int uti = s.Count(x => x.Effects.HasUtility || x.Effects.HasVision || x.Effects.HasZoneControl);
    return new HeroKitProfile(d, cc, mob, sus, uti, s.Any(x => x.Effects.HasSummon), s.Any(x => x.Effects.HasTransformation), s.Any(x => x.Effects.HasExecute), d >= BurstDamageThreshold && !s.Any(x => x.Effects.HasDamageOverTime), sus >= SustainThreshold, cc >= ControlThreshold, mob >= MobilityThreshold);
  }
}
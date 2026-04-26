using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Versioned.Snapshotting;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HeroService.Application.Heroes.Versioned.Snapshotting;

/// <summary>
/// SnapshotResolver is the deterministic combat simulation engine of the Hero system.
///
///– It compiles raw domain data into immutable runtime snapshots.
/// </summary>
public class SnapshotResolver
{
  /// <summary>
  /// Resolves a complete HeroSnapshot from raw domain inputs.
  /// </summary>
  public HeroSnapshot ResolveHero(
      Guid heroId,
      Guid gameVersionId,
      HeroBaseStats baseStats,
      HeroModifier? heroModifier,
      IReadOnlyList<Skill> skills,
      IReadOnlyDictionary<Guid, SkillBaseStats> skillBaseStats,
      IReadOnlyDictionary<Guid, SkillModifier?> skillModifiers)
  {
    var skillSnapshots = new List<SkillSnapshot>();

    foreach (var skill in skills)
    {
      var baseStat = skillBaseStats[skill.Id];
      skillModifiers.TryGetValue(skill.Id, out var modifier);

      skillSnapshots.Add(BuildSkillSnapshot(
        skill.Id,
        gameVersionId,
        baseStat,
        modifier,
        skill.Effects
      ));
    }

    return new HeroSnapshot(
        heroId,
        gameVersionId,
        Apply(baseStats.BaseHealth, heroModifier?.HealthMultiplier),
        Apply(baseStats.BaseMana, heroModifier?.ManaMultiplier),
        Apply(baseStats.BaseAttackDamage, heroModifier?.AttackDamageMultiplier),
        Apply(baseStats.BaseAbilityPower, heroModifier?.AbilityPowerMultiplier),
        Apply(baseStats.BaseAttackSpeed, heroModifier?.AttackSpeedMultiplier),
        Apply(baseStats.BaseCritChance, heroModifier?.CritChanceMultiplier),
        Apply(baseStats.BaseCritDamageMultiplier, heroModifier?.CritDamageMultiplier),
        Apply(baseStats.BaseArmor, heroModifier?.ArmorMultiplier),
        Apply(baseStats.BaseMagicResistance, heroModifier?.MagicResistanceMultiplier),
        Apply(baseStats.BaseDamageReduction, heroModifier?.DamageReductionMultiplier),
        Apply(baseStats.BaseMovementSpeed, heroModifier?.MovementSpeedMultiplier),
        Apply(baseStats.BaseAttackRange, heroModifier?.AttackRangeMultiplier),
        Apply(baseStats.BaseCastSpeed, heroModifier?.CastSpeedMultiplier),
        Apply(baseStats.BaseCooldownReduction, heroModifier?.CooldownReductionMultiplier),
        Apply(baseStats.BaseResourceRegeneration, heroModifier?.ResourceRegenerationMultiplier),
        skillSnapshots
    );
  }

  /// <summary>
  /// Builds a deterministic SkillSnapshot from domain inputs.
  /// </summary>
  private static SkillSnapshot BuildSkillSnapshot(
   Guid skillId,
   Guid gameVersionId,
   SkillBaseStats baseStats,
   SkillModifier? modifier,
   IReadOnlyCollection<SkillEffect> effects)
  {
    var cooldown = Apply(baseStats.BaseCooldown, modifier?.CooldownMultiplier);
    var mana = Apply(baseStats.BaseManaCost, modifier?.ManaCostMultiplier);
    var damage = Apply(baseStats.BaseDamage, modifier?.DamageMultiplier);

    // ----------------------------
    // EFFECT CLASSIFICATION (SOURCE OF TRUTH)
    // ----------------------------

    bool hasDamage =
      effects.Any(e =>
        e.EffectType is EffectType.Damage or EffectType.DamageOverTime);

    bool hasHealing =
      effects.Any(e =>
        e.EffectType is EffectType.Heal or EffectType.HealOverTime);

    bool hasShielding =
      effects.Any(e =>
        e.EffectType == EffectType.Shield);

    bool hasCrowdControl =
      effects.Any(e =>
        e.EffectType == EffectType.CrowdControl);

    bool hasMobility =
      effects.Any(e =>
        e.EffectType == EffectType.Mobility);

    bool isBuff =
      effects.Any(e =>
        e.EffectType == EffectType.Buff);

    bool isDebuff =
      effects.Any(e =>
        e.EffectType == EffectType.Debuff);

    bool isUltimate =
      effects.Any(e =>
        e.EffectType == EffectType.Execute);

    // ----------------------------
    // CC DURATION (ONLY IF APPLICABLE)
    // ----------------------------

    float ccDuration = hasCrowdControl
      ? baseStats.BaseCrowdControlDuration
      : 0f;

    return new SkillSnapshot(
      skillId,
      gameVersionId,
      cooldown,
      mana,
      damage,
      baseStats.BaseHealing,
      baseStats.BaseShieldValue,
      baseStats.BaseCastTime,
      baseStats.BaseChannelDuration,
      baseStats.BaseRange,
      baseStats.AttackDamageRatio,
      baseStats.AbilityPowerRatio,
      baseStats.MaxHealthRatio,
      hasDamage,
      hasHealing,
      hasShielding,
      hasCrowdControl,
      hasMobility,
      isBuff,
      isDebuff,
      isUltimate,
      ccDuration
    );
  }

  private static float Apply(float baseValue, float? multiplier)
    => baseValue * (multiplier ?? 1f);
}
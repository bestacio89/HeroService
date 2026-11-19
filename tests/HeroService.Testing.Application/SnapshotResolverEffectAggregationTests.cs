using FluentAssertions;
using HeroService.Application.Heroes.Versioned.Snapshotting;
using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Skills;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Testing.Application;

public sealed class SnapshotResolverEffectAggregationTests
{
  [Fact]
  public void ResolveHero_PreservesEveryEffectTypeAndModifierClassification()
  {
    var version = new TestGameVersion(Guid.NewGuid());
    version.Define("1.0.0", "Test version", "unit-test");
    version.Activate("unit-test");

    var heroId = Guid.NewGuid();
    var heroModifier = CreateHeroModifier(version.Id, heroId);
    var baseStats = CreateHeroBaseStats(heroId);
    var skills = Enumerable.Range(0, 5)
        .Select(index => CreateSkill($"Skill {index}"))
        .ToArray();

    foreach (var effectType in Enum.GetValues<EffectType>())
    {
      var effect = new TestSkillEffect(Guid.NewGuid());
      effect.Define(
          skills[0].Id,
          effectType,
          magnitude: 1f,
          duration: 1f,
          radius: 1f,
          targetType: default,
          stackType: default,
          maxStacks: 1,
          adRatio: null,
          mdRatio: null,
          hpRatio: null,
          isPeriodic: false,
          isInstant: false,
          isChannelled: false,
          buffType: effectType == EffectType.Buff ? BuffType.Armor : null,
          debuffType: effectType == EffectType.Debuff ? DebuffType.ArmorReduction : null,
          createdBy: "unit-test");

      skills[0].AddEffect(effect);
    }

    var baseStatsBySkill = skills.ToDictionary(
        skill => skill.Id,
        skill => CreateSkillBaseStats(skill.Id));
    var modifiersBySkill = skills.ToDictionary(
        skill => skill.Id,
        skill => CreateSkillModifier(version.Id, skill.Id));

    var snapshot = new SnapshotResolver().ResolveHero(
        heroId,
        "Test Hero",
        version,
        baseStats,
        heroModifier,
        skills,
        baseStatsBySkill,
        modifiersBySkill);

    var passive = snapshot.SkillKit.Passive;
    var effects = passive.Effects;

    var flags = new Dictionary<EffectType, bool>
    {
      [EffectType.Damage] = effects.HasDamage,
      [EffectType.DamageOverTime] = effects.HasDamageOverTime,
      [EffectType.Heal] = effects.HasHeal,
      [EffectType.HealOverTime] = effects.HasHealOverTime,
      [EffectType.Shield] = effects.HasShield,
      [EffectType.Slow] = effects.HasSlow,
      [EffectType.Root] = effects.HasRoot,
      [EffectType.Stun] = effects.HasStun,
      [EffectType.Silence] = effects.HasSilence,
      [EffectType.Disarm] = effects.HasDisarm,
      [EffectType.Blind] = effects.HasBlind,
      [EffectType.Fear] = effects.HasFear,
      [EffectType.Charm] = effects.HasCharm,
      [EffectType.Taunt] = effects.HasTaunt,
      [EffectType.Confuse] = effects.HasConfuse,
      [EffectType.Sleep] = effects.HasSleep,
      [EffectType.Knockback] = effects.HasKnockback,
      [EffectType.KnockUp] = effects.HasKnockUp,
      [EffectType.Pull] = effects.HasPull,
      [EffectType.Freeze] = effects.HasFreeze,
      [EffectType.Petrify] = effects.HasPetrify,
      [EffectType.Buff] = effects.HasBuff,
      [EffectType.Debuff] = effects.HasDebuff,
      [EffectType.Mobility] = effects.HasMobility,
      [EffectType.Execute] = effects.HasExecute,
      [EffectType.Utility] = effects.HasUtility,
      [EffectType.Vision] = effects.HasVision,
      [EffectType.ZoneControl] = effects.HasZoneControl,
      [EffectType.Summon] = effects.HasSummon,
      [EffectType.Transformation] = effects.HasTransformation
    };

    flags.Should().HaveCount(Enum.GetValues<EffectType>().Length);
    flags.Should().OnlyContain(entry => entry.Value);
    passive.EffectExecutions.Should().HaveCount(Enum.GetValues<EffectType>().Length);
    effects.BuffTypes.Should().Contain(BuffType.Armor);
    effects.DebuffTypes.Should().Contain(DebuffType.ArmorReduction);
  }

  private static TestSkill CreateSkill(string name)
  {
    var skill = new TestSkill(Guid.NewGuid());
    skill.Define(name, default, "unit-test");
    return skill;
  }

  private static TestHeroBaseStats CreateHeroBaseStats(Guid heroId)
  {
    var stats = new TestHeroBaseStats(Guid.NewGuid());
    stats.Define(
        heroId,
        1f, 1f,
        1f, 1f,
        0f,
        1f, 1f,
        1f, 1f,
        1f, 1f, 1f,
        1f,
        1f, 1f,
        1f, 1f,
        "unit-test");
    return stats;
  }

  private static TestSkillBaseStats CreateSkillBaseStats(Guid skillId)
  {
    var stats = new TestSkillBaseStats(Guid.NewGuid());
    stats.Define(
        skillId,
        1f, 1f,
        1f, 1f, 1f,
        1f, 1f,
        1f, 1f,
        "unit-test");
    return stats;
  }

  private static TestSkillModifier CreateSkillModifier(
      Guid versionId,
      Guid skillId)
  {
    var modifier = new TestSkillModifier(Guid.NewGuid());
    modifier.Define(
        versionId,
        skillId,
        1f, 1f,
        1f, 1f, 1f,
        1f, 1f,
        1f, 1f,
        "unit-test");
    return modifier;
  }

  private static TestHeroModifier CreateHeroModifier(
      Guid versionId,
      Guid heroId)
  {
    var modifier = new TestHeroModifier(Guid.NewGuid());
    modifier.Define(
        versionId,
        heroId,
        1f, 1f,
        1f, 1f,
        0f,
        1f, 1f,
        1f, 1f,
        1f, 1f, 1f,
        1f,
        1f, 1f,
        1f, 1f,
        "unit-test");
    return modifier;
  }

  private sealed class TestGameVersion(Guid id) : GameVersion(id);

  private sealed class TestSkill(Guid id) : Skill(id);

  private sealed class TestSkillEffect(Guid id) : SkillEffect(id);

  private sealed class TestHeroBaseStats(Guid id) : HeroBaseStats(id);

  private sealed class TestSkillBaseStats(Guid id) : SkillBaseStats(id);

  private sealed class TestSkillModifier(Guid id) : SkillModifier(id);

  private sealed class TestHeroModifier(Guid id) : HeroModifier(id);
}

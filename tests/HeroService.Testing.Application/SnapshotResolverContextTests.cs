using HeroService.Application.Heroes.Versioned.Snapshotting;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Testing.Application;

public sealed class SnapshotResolverContextTests
{
  [Fact]
  public void ResolveHero_RejectsInactiveGameVersion()
  {
    var version = CreateVersion();
    var resolver = new SnapshotResolver();

    var exception = Assert.Throws<InvalidOperationException>(() =>
      resolver.ResolveHero(
        Guid.NewGuid(),
        "Test Hero",
        version,
        baseStats: null!,
        heroModifier: null!,
        skills: Array.Empty<Skill>(),
        skillBaseStats: new Dictionary<Guid, SkillBaseStats>(),
        skillModifiers: new Dictionary<Guid, SkillModifier>()));

    Assert.Contains("is not active", exception.Message, StringComparison.Ordinal);
  }

  [Fact]
  public void ResolveHero_RejectsModifierFromDifferentGameVersion()
  {
    var version = CreateVersion();
    version.Activate("unit-test");
    var modifier = CreateModifier(Guid.NewGuid(), Guid.NewGuid());
    var resolver = new SnapshotResolver();

    var exception = Assert.Throws<InvalidOperationException>(() =>
      resolver.ResolveHero(
        Guid.NewGuid(),
        "Test Hero",
        version,
        baseStats: null!,
        heroModifier: modifier,
        skills: Array.Empty<Skill>(),
        skillBaseStats: new Dictionary<Guid, SkillBaseStats>(),
        skillModifiers: new Dictionary<Guid, SkillModifier>()));

    Assert.Contains("does not belong to active version", exception.Message, StringComparison.Ordinal);
  }

  private static TestGameVersion CreateVersion()
  {
    var version = new TestGameVersion(Guid.NewGuid());
    version.Define("1.0", "Test version", "unit-test");
    return version;
  }

  private static TestHeroModifier CreateModifier(Guid versionId, Guid heroId)
  {
    var modifier = new TestHeroModifier(Guid.NewGuid());
    modifier.Define(
      versionId,
      heroId,
      healthMultiplier: 1f,
      manaMultiplier: 1f,
      attackDamageMultiplier: 1f,
      magicDamageMultiplier: 1f,
      ignoreEnemyDefenseAdjustment: 0f,
      attackSpeedMultiplier: 1f,
      castSpeedMultiplier: 1f,
      critChanceMultiplier: 1f,
      critDamageMultiplier: 1f,
      armorMultiplier: 1f,
      magicResistanceMultiplier: 1f,
      damageReductionMultiplier: 1f,
      shieldStrengthMultiplier: 1f,
      movementSpeedMultiplier: 1f,
      attackRangeMultiplier: 1f,
      cooldownReductionMultiplier: 1f,
      resourceRegenerationMultiplier: 1f,
      createdBy: "unit-test");
    return modifier;
  }

  private sealed class TestGameVersion : GameVersion
  {
    public TestGameVersion(Guid id) : base(id) { }
  }

  private sealed class TestHeroModifier : HeroModifier
  {
    public TestHeroModifier(Guid id) : base(id) { }
  }
}

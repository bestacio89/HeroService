using HeroService.Domain.Heroes.Core;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using HeroService.Domain.Heroes.Versioned.GameVersion.Modifiers;

namespace HeroService.Testing.Domain;

public sealed class DomainInvariantTests
{
  [Theory]
  [InlineData("", "Release 1")]
  [InlineData("   ", "Release 1")]
  public void GameVersion_Define_RejectsBlankVersionNumber(string versionNumber, string versionName)
  {
    var sut = new TestGameVersion(Guid.NewGuid());

    Assert.Throws<ArgumentException>(() => sut.Define(versionNumber, versionName, "unit-test"));
  }

  [Theory]
  [InlineData("1.0", "")]
  [InlineData("1.0", "   ")]
  public void GameVersion_Define_RejectsBlankVersionName(string versionNumber, string versionName)
  {
    var sut = new TestGameVersion(Guid.NewGuid());

    Assert.Throws<ArgumentException>(() => sut.Define(versionNumber, versionName, "unit-test"));
  }

  [Fact]
  public void GameVersion_Define_CannotBeAppliedTwice()
  {
    var sut = new TestGameVersion(Guid.NewGuid());
    sut.Define("1.0", "First release", "unit-test");

    Assert.Throws<InvalidOperationException>(() => sut.Define("1.1", "Second release", "unit-test"));
  }

  [Fact]
  public void GameVersion_ActivationAndDeactivation_AreIdempotent()
  {
    var sut = new TestGameVersion(Guid.NewGuid());
    sut.Define("1.0", "First release", "unit-test");

    Assert.False(sut.IsActive);

    sut.Activate("unit-test");
    sut.Activate("unit-test");
    Assert.True(sut.IsActive);

    sut.Deactivate("unit-test");
    sut.Deactivate("unit-test");
    Assert.False(sut.IsActive);
  }

  [Theory]
  [InlineData(0f)]
  [InlineData(1f)]
  public void HeroBaseStats_Define_AcceptsIgnoreDefenseBoundaries(float ignoreEnemyDefense)
  {
    var sut = new TestHeroBaseStats(Guid.NewGuid());

    DefineStats(sut, Guid.NewGuid(), ignoreEnemyDefense);

    Assert.Equal(ignoreEnemyDefense, sut.BaseIgnoreEnemyDefense);
  }

  [Theory]
  [InlineData(-0.01f)]
  [InlineData(1.01f)]
  public void HeroBaseStats_Define_RejectsIgnoreDefenseOutsideRange(float ignoreEnemyDefense)
  {
    var sut = new TestHeroBaseStats(Guid.NewGuid());

    Assert.Throws<ArgumentOutOfRangeException>(() => DefineStats(sut, Guid.NewGuid(), ignoreEnemyDefense));
  }

  [Fact]
  public void HeroBaseStats_Define_RejectsEmptyHeroId()
  {
    var sut = new TestHeroBaseStats(Guid.NewGuid());

    Assert.Throws<ArgumentException>(() => DefineStats(sut, Guid.Empty, 0.5f));
  }

  [Fact]
  public void HeroModifier_Define_RejectsEmptyGameVersionId()
  {
    var sut = new TestHeroModifier(Guid.NewGuid());

    Assert.Throws<ArgumentException>(() => DefineModifier(sut, Guid.Empty, Guid.NewGuid(), 0f));
  }

  [Fact]
  public void HeroModifier_Define_RejectsEmptyHeroId()
  {
    var sut = new TestHeroModifier(Guid.NewGuid());

    Assert.Throws<ArgumentException>(() => DefineModifier(sut, Guid.NewGuid(), Guid.Empty, 0f));
  }

  [Theory]
  [InlineData(-1f)]
  [InlineData(1f)]
  public void HeroModifier_Define_AcceptsIgnoreDefenseAdjustmentBoundaries(float adjustment)
  {
    var sut = new TestHeroModifier(Guid.NewGuid());

    DefineModifier(sut, Guid.NewGuid(), Guid.NewGuid(), adjustment);

    Assert.Equal(adjustment, sut.IgnoreEnemyDefenseAdjustment);
  }

  [Theory]
  [InlineData(-1.01f)]
  [InlineData(1.01f)]
  public void HeroModifier_Define_RejectsIgnoreDefenseAdjustmentOutsideRange(float adjustment)
  {
    var sut = new TestHeroModifier(Guid.NewGuid());

    Assert.Throws<ArgumentOutOfRangeException>(() =>
      DefineModifier(sut, Guid.NewGuid(), Guid.NewGuid(), adjustment));
  }

  private static void DefineStats(HeroBaseStats stats, Guid heroId, float ignoreEnemyDefense)
  {
    stats.Define(
      heroId,
      baseHealth: 100f,
      baseMana: 100f,
      baseAttackDamage: 20f,
      baseMagicDamage: 20f,
      baseIgnoreEnemyDefense: ignoreEnemyDefense,
      baseAttackSpeed: 1f,
      baseCastSpeed: 1f,
      baseCritChance: 0.05f,
      baseCritDamageMultiplier: 2f,
      baseArmor: 10f,
      baseMagicResistance: 10f,
      baseDamageReduction: 0f,
      baseShieldStrengthMultiplier: 1f,
      baseMovementSpeed: 300f,
      baseAttackRange: 550f,
      baseCooldownReduction: 0.1f,
      baseResourceRegeneration: 1f,
      createdBy: "unit-test");
  }

  private static void DefineModifier(
    HeroModifier modifier,
    Guid gameVersionId,
    Guid heroId,
    float ignoreEnemyDefenseAdjustment)
  {
    modifier.Define(
      gameVersionId,
      heroId,
      healthMultiplier: 1f,
      manaMultiplier: 1f,
      attackDamageMultiplier: 1f,
      magicDamageMultiplier: 1f,
      ignoreEnemyDefenseAdjustment: ignoreEnemyDefenseAdjustment,
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
  }

  private sealed class TestGameVersion : GameVersion
  {
    public TestGameVersion(Guid id) : base(id) { }
  }

  private sealed class TestHeroBaseStats : HeroBaseStats
  {
    public TestHeroBaseStats(Guid id) : base(id) { }
  }

  private sealed class TestHeroModifier : HeroModifier
  {
    public TestHeroModifier(Guid id) : base(id) { }
  }
}

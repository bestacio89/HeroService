using FluentAssertions;
using HeroService.Application.Heroes.Versioned.Snapshotting;

namespace HeroService.Testing.Application;

public sealed class HeroSnapshotCachingTests
{
  [Fact]
  public void SingleVersionKey_SeparatesTheSameHeroAcrossGameVersions()
  {
    var heroId = Guid.NewGuid();
    var firstVersionId = Guid.NewGuid();
    var secondVersionId = Guid.NewGuid();

    var firstKey = HeroSnapshotCaching.SingleVersionKey(firstVersionId, heroId);
    var secondKey = HeroSnapshotCaching.SingleVersionKey(secondVersionId, heroId);

    firstKey.Should().NotBe(secondKey);
    firstKey.Should().Contain(firstVersionId.ToString());
    firstKey.Should().Contain(heroId.ToString());
    secondKey.Should().Contain(secondVersionId.ToString());
    secondKey.Should().Contain(heroId.ToString());
  }

  [Fact]
  public void BrowseVersionKey_IsStableForTheSameGameVersion()
  {
    var versionId = Guid.NewGuid();

    HeroSnapshotCaching.BrowseVersionKey(versionId)
        .Should()
        .Be(HeroSnapshotCaching.BrowseVersionKey(versionId));
  }

  [Fact]
  public void BrowseVersionKey_SeparatesDifferentGameVersions()
  {
    var firstVersionId = Guid.NewGuid();
    var secondVersionId = Guid.NewGuid();

    HeroSnapshotCaching.BrowseVersionKey(firstVersionId)
        .Should()
        .NotBe(HeroSnapshotCaching.BrowseVersionKey(secondVersionId));
  }
}

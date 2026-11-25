using FluentAssertions;
using HeroService.Admin.Components.Pages.Admin.Heroes;
using HeroService.Admin.Components.Pages.Admin.Skills;
using HeroService.Domain.Heroes.Skills;

namespace HeroService.Testing.Admin.Presentation;

public sealed class SemanticIconThemeTests
{
  [Fact]
  public void SkillTheme_ShouldProvideAnIconForEveryDefinedSkillType()
  {
    foreach (var type in Enum.GetValues<SkillType>())
      SkillTheme.GetIcon(type).Should().NotBeNullOrWhiteSpace();
  }

  [Fact]
  public void SkillEffectTheme_ShouldProvideAnIconForEveryDefinedEffectType()
  {
    foreach (var type in Enum.GetValues<EffectType>())
      SkillEffectTheme.GetIcon(type).Should().NotBeNullOrWhiteSpace();
  }

  [Fact]
  public void HeroClassTheme_ShouldResolveAllSeededClasses()
  {
    var classes = new[] { "Warrior", "Assassin", "Mage", "Tank", "Support", "Ranger" };

    foreach (var className in classes)
      HeroClassTheme.GetIcon(className).Should().NotBeNullOrWhiteSpace();
  }

  [Theory]
  [InlineData("  WARRIOR  ")]
  [InlineData("mage")]
  [InlineData("Support")]
  [InlineData("Ranger")]
  public void HeroClassTheme_ShouldNormalizeClassNames(string className)
  {
    HeroClassTheme.GetIcon(className).Should().NotBe(HeroClassTheme.GetIcon("unrecognized-class"));
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("unrecognized-class")]
  public void HeroClassTheme_ShouldUseNeutralFallbackForUnknownNames(string? className)
  {
    HeroClassTheme.GetIcon(className).Should().Be(MudBlazor.Icons.Material.Filled.PersonOutline);
  }
}

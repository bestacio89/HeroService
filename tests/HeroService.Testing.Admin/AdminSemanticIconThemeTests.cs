using FluentAssertions;
using HeroService.Admin.Components.Pages.Admin.Heroes;
using HeroService.Admin.Components.Pages.Admin.Skills;
using HeroService.Domain.Heroes.Skills;
using MudBlazor;

namespace HeroService.Testing.Admin;

public sealed class AdminSemanticIconThemeTests
{
    [Fact]
    public void SkillTheme_MapsEveryDefinedSkillTypeToASemanticIcon()
    {
        var mappings = new Dictionary<SkillType, string>
        {
            [SkillType.Damage] = Icons.Material.Filled.Whatshot,
            [SkillType.Heal] = Icons.Material.Filled.Healing,
            [SkillType.Shield] = Icons.Material.Filled.Shield,
            [SkillType.Mobility] = Icons.Material.Filled.DirectionsRun,
            [SkillType.Control] = Icons.Material.Filled.ControlCamera,
            [SkillType.Buff] = Icons.Material.Filled.TrendingUp,
            [SkillType.Debuff] = Icons.Material.Filled.TrendingDown,
            [SkillType.Ultimate] = Icons.Material.Filled.AutoAwesome
        };

        Enum.GetValues<SkillType>().Should().BeEquivalentTo(mappings.Keys);
        mappings.Should().OnlyContain(mapping => SkillTheme.GetIcon(mapping.Key) == mapping.Value);
    }

    [Fact]
    public void SkillTheme_UsesNeutralIconForUnknownEnumValue()
    {
        SkillTheme.GetIcon((SkillType)int.MaxValue).Should().Be(Icons.Material.Filled.Help);
    }

    [Fact]
    public void SkillEffectTheme_MapsEveryDefinedEffectToASemanticIcon()
    {
        var mappings = new Dictionary<EffectType, string>
        {
            [EffectType.Damage] = Icons.Material.Filled.Bolt,
            [EffectType.DamageOverTime] = Icons.Material.Filled.LocalFireDepartment,
            [EffectType.Heal] = Icons.Material.Filled.Healing,
            [EffectType.HealOverTime] = Icons.Material.Filled.MedicalServices,
            [EffectType.Shield] = Icons.Material.Filled.Shield,
            [EffectType.Slow] = Icons.Material.Filled.SlowMotionVideo,
            [EffectType.Root] = Icons.Material.Filled.Lock,
            [EffectType.Stun] = Icons.Material.Filled.FlashOn,
            [EffectType.Silence] = Icons.Material.Filled.VolumeOff,
            [EffectType.Disarm] = Icons.Material.Filled.PanTool,
            [EffectType.Blind] = Icons.Material.Filled.VisibilityOff,
            [EffectType.Fear] = Icons.Material.Filled.SentimentVeryDissatisfied,
            [EffectType.Charm] = Icons.Material.Filled.Favorite,
            [EffectType.Taunt] = Icons.Material.Filled.Campaign,
            [EffectType.Confuse] = Icons.Material.Filled.Help,
            [EffectType.Sleep] = Icons.Material.Filled.Bedtime,
            [EffectType.Knockback] = Icons.Material.Filled.ArrowForward,
            [EffectType.KnockUp] = Icons.Material.Filled.VerticalAlignTop,
            [EffectType.Pull] = Icons.Material.Filled.MoveDown,
            [EffectType.Freeze] = Icons.Material.Filled.AcUnit,
            [EffectType.Petrify] = Icons.Material.Filled.Terrain,
            [EffectType.Buff] = Icons.Material.Filled.TrendingUp,
            [EffectType.Debuff] = Icons.Material.Filled.TrendingDown,
            [EffectType.Mobility] = Icons.Material.Filled.DirectionsRun,
            [EffectType.Execute] = Icons.Material.Filled.Gavel,
            [EffectType.Utility] = Icons.Material.Filled.Build,
            [EffectType.Vision] = Icons.Material.Filled.Visibility,
            [EffectType.ZoneControl] = Icons.Material.Filled.Radar,
            [EffectType.Summon] = Icons.Material.Filled.Pets,
            [EffectType.Transformation] = Icons.Material.Filled.AutoFixHigh
        };

        Enum.GetValues<EffectType>().Should().BeEquivalentTo(mappings.Keys);
        mappings.Should().OnlyContain(mapping => SkillEffectTheme.GetIcon(mapping.Key) == mapping.Value);
    }

    [Fact]
    public void SkillEffectTheme_UsesNeutralIconForUnknownEnumValue()
    {
        SkillEffectTheme.GetIcon((EffectType)int.MaxValue).Should().Be(Icons.Material.Filled.Help);
    }

    [Theory]
    [InlineData("Assassin", "VisibilityOff")]
    [InlineData("Mage", "AutoAwesome")]
    [InlineData("Ranger", "GpsFixed")]
    [InlineData("Support", "Favorite")]
    [InlineData("Tank", "Shield")]
    [InlineData("Warrior", "FitnessCenter")]
    [InlineData("  assassin  ", "VisibilityOff")]
    [InlineData("Marksman", "GpsFixed")]
    [InlineData("Guardian", "Shield")]
    [InlineData("Unknown class", "HelpOutline")]
    [InlineData(null, "HelpOutline")]
    public void HeroClassTheme_ResolvesKnownNamesAliasesAndFallback(string? className, string expectedIcon)
    {
        var expected = expectedIcon switch
        {
            "VisibilityOff" => Icons.Material.Filled.VisibilityOff,
            "AutoAwesome" => Icons.Material.Filled.AutoAwesome,
            "GpsFixed" => Icons.Material.Filled.GpsFixed,
            "Favorite" => Icons.Material.Filled.Favorite,
            "Shield" => Icons.Material.Filled.Shield,
            "FitnessCenter" => Icons.Material.Filled.FitnessCenter,
            _ => Icons.Material.Filled.HelpOutline
        };

        HeroClassTheme.GetIcon(className).Should().Be(expected);
    }
}

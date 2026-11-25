using Bunit;
using FluentAssertions;
using HeroService.Admin.Components.Pages;
using HeroService.Client.Http.Abstractions;
using HeroService.Contracts.DTOs.GameVersions;
using HeroService.Contracts.DTOs.Heroes;
using HeroService.Contracts.DTOs.Skills;
using HeroService.Domain.Heroes.Skills;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;

namespace HeroService.Testing.Admin;

public sealed class HomeDashboardTests
{
    [Fact]
    public void Home_DisplaysCatalogueMetricsAndActiveGameVersion()
    {
        using var context = CreateContext(
            [
                CreateHero("Astra", "Guardian"),
                CreateHero("Boreas", "Guardian"),
                CreateHero("Cinder", "Striker")
            ],
            [
                CreateSkill("Shield Wall", SkillType.Shield),
                CreateSkill("Arcane Bolt", SkillType.Damage),
                CreateSkill("Healing Wave", SkillType.Heal)
            ],
            new GameVersionDto(Guid.NewGuid(), "1.4.0", "The Founders' Update", true));

        var cut = context.RenderComponent<Home>();

        cut.WaitForAssertion(() =>
        {
            cut.Find("[data-testid='hero-count']").TextContent.Should().Be("3");
            cut.Find("[data-testid='skill-count']").TextContent.Should().Be("3");
            cut.Find("[data-testid='active-game-version']").TextContent.Should().Be("1.4.0");
            cut.FindAll(".dashboard-distribution-row").Should().HaveCount(5);
            cut.Markup.Should().Contain("Guardian");
            cut.Markup.Should().Contain("Damage");
        });
    }

    [Fact]
    public void Home_KeepsSuccessfulCatalogueDataVisibleWhenAnotherSourceFails()
    {
        using var context = CreateContext(
            [CreateHero("Astra", "Guardian")],
            [
                CreateSkill("Shield Wall", SkillType.Shield),
                CreateSkill("Arcane Bolt", SkillType.Damage)
            ],
            null,
            failHeroRequest: true);

        var cut = context.RenderComponent<Home>();

        cut.WaitForAssertion(() =>
        {
            cut.Find("[data-testid='hero-count']").TextContent.Should().Be("Unavailable");
            cut.Find("[data-testid='skill-count']").TextContent.Should().Be("2");
            cut.Find("[data-testid='active-game-version']").TextContent.Should().Be("Not configured");
            cut.Markup.Should().Contain("Hero catalogue data could not be loaded.");
        });
    }

    private static TestContext CreateContext(
        IReadOnlyCollection<HeroDto> heroes,
        IReadOnlyCollection<SkillDto> skills,
        GameVersionDto? activeGameVersion,
        bool failHeroRequest = false)
    {
        var context = new TestContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddMudServices();
        context.Services.AddLogging();

        var heroClient = new Mock<IHeroClient>();
        if (failHeroRequest)
        {
            heroClient
                .Setup(client => client.BrowseAsync(
                    null,
                    null,
                    null,
                    null,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new HttpRequestException("API unavailable"));
        }
        else
        {
            heroClient
                .Setup(client => client.BrowseAsync(
                    null,
                    null,
                    null,
                    null,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(heroes);
        }

        var skillClient = new Mock<ISkillClient>();
        skillClient
            .Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(skills);

        var gameVersionClient = new Mock<IGameVersionClient>();
        gameVersionClient
            .Setup(client => client.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeGameVersion);

        context.Services.AddSingleton(heroClient.Object);
        context.Services.AddSingleton(skillClient.Object);
        context.Services.AddSingleton(gameVersionClient.Object);

        return context;
    }

    private static HeroDto CreateHero(string name, string className)
    {
        return new HeroDto(
            Guid.NewGuid(),
            name,
            Guid.NewGuid(),
            new HeroClassDto(Guid.NewGuid(), className),
            null!,
            null!,
            null!);
    }

    private static SkillDto CreateSkill(string name, SkillType type)
    {
        return new SkillDto
        {
            Id = Guid.NewGuid(),
            Name = name,
            SkillType = type
        };
    }
}

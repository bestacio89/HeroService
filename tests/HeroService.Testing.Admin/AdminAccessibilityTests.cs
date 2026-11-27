using Bunit;
using FluentAssertions;
using HeroService.Admin.Components.Layout;
using HeroService.Admin.Components.Pages;
using HeroService.Admin.UI;
using HeroService.Admin.Operational;
using HeroService.Client.Http.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Services;

namespace HeroService.Testing.Admin;

public sealed class AdminAccessibilityTests
{
    [Fact]
    public void Home_ProvidesOnePageHeadingAndNamedSections()
    {
        using var context = CreateContext();

        var cut = context.RenderComponent<Home>();

        cut.FindAll("h1").Should().ContainSingle();
        cut.Find("h1").TextContent.Should().Be("World command center");
        cut.Find("section[aria-labelledby='catalogue-overview-title']").Should().NotBeNull();
        cut.Find("section[aria-labelledby='catalogue-composition-title']").Should().NotBeNull();
        cut.Find("section[aria-labelledby='core-authoring-title']").Should().NotBeNull();
        cut.Find("section[aria-labelledby='balance-tuning-title']").Should().NotBeNull();
        cut.Find("section[aria-labelledby='world-classification-title']").Should().NotBeNull();
    }

    [Fact]
    public void MainLayout_ProvidesSkipLinkAndNamedNavigationLandmarks()
    {
        using var context = CreateContext();

        var cut = context.RenderComponent<MainLayout>(parameters => parameters
            .Add(
                layout => layout.Body,
                builder => builder.AddMarkupContent(0, "<h1>Test page</h1>")));

        cut.Find("a.skip-link").GetAttribute("href").Should().Be("#main-content");
        cut.Find("nav[aria-label='Primary navigation']").Should().NotBeNull();
        cut.Find("#main-content").GetAttribute("tabindex").Should().Be("-1");
        cut.Find("#main-content").GetAttribute("role").Should().Be("main");
        cut.Find("button[aria-label='Toggle navigation']")
            .GetAttribute("aria-controls")
            .Should().Be("primary-navigation");
    }

    private static TestContext CreateContext()
    {
        var context = new TestContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddMudServices();
        context.Services.AddLogging();
        context.Services.AddSingleton(GameTheme.Create());

        var operationalStatusClient = new Mock<IOperationalStatusClient>();
        operationalStatusClient
            .Setup(client => client.GetStatusesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<OperationalCheckResult>());
        context.Services.AddSingleton(operationalStatusClient.Object);

        var heroClient = new Mock<IHeroClient>();
        heroClient
            .Setup(client => client.BrowseAsync(
                null,
                null,
                null,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<HeroDto>());

        var skillClient = new Mock<ISkillClient>();
        skillClient
            .Setup(client => client.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var gameVersionClient = new Mock<IGameVersionClient>();
        gameVersionClient
            .Setup(client => client.GetActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((HeroService.Contracts.DTOs.GameVersions.GameVersionDto?)null);

        context.Services.AddSingleton(heroClient.Object);
        context.Services.AddSingleton(skillClient.Object);
        context.Services.AddSingleton(gameVersionClient.Object);

        return context;
    }
}

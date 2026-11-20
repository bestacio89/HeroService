using Bunit;
using HeroService.Admin.Components.Layout;
using HeroService.Admin.Components.Pages;
using HeroService.Admin.UI;
using Microsoft.Extensions.DependencyInjection;
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
        cut.Find("h1").TextContent.Should().Be("Codex workspace");
        cut.Find("section[aria-labelledby='core-authoring-title']");
        cut.Find("section[aria-labelledby='balance-tuning-title']");
        cut.Find("section[aria-labelledby='world-classification-title']");
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
        cut.Find("nav[aria-label='Primary navigation']");
        cut.Find("main#main-content").GetAttribute("tabindex").Should().Be("-1");
        cut.Find("button[aria-label='Toggle navigation']").GetAttribute("aria-controls")
            .Should().Be("primary-navigation");
    }

    private static TestContext CreateContext()
    {
        var context = new TestContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddMudServices();
        context.Services.AddLogging();
        context.Services.AddSingleton(GameTheme.Create());
        return context;
    }
}

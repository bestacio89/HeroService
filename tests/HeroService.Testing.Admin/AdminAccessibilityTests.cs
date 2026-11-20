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

        Assert.Single(cut.FindAll("h1"));
        Assert.Equal("Codex workspace", cut.Find("h1").TextContent);
        Assert.NotNull(cut.Find("section[aria-labelledby='core-authoring-title']"));
        Assert.NotNull(cut.Find("section[aria-labelledby='balance-tuning-title']"));
        Assert.NotNull(cut.Find("section[aria-labelledby='world-classification-title']"));
    }

    [Fact]
    public void MainLayout_ProvidesSkipLinkAndNamedNavigationLandmarks()
    {
        using var context = CreateContext();

        var cut = context.RenderComponent<MainLayout>(parameters => parameters
            .Add(
                layout => layout.Body,
                builder => builder.AddMarkupContent(0, "<h1>Test page</h1>")));

        Assert.Equal("#main-content", cut.Find("a.skip-link").GetAttribute("href"));
        Assert.NotNull(cut.Find("nav[aria-label='Primary navigation']"));
        Assert.Equal("-1", cut.Find("#main-content").GetAttribute("tabindex"));
        Assert.Equal("main", cut.Find("#main-content").GetAttribute("role"));
        Assert.Equal(
            "primary-navigation",
            cut.Find("button[aria-label='Toggle navigation']").GetAttribute("aria-controls"));
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

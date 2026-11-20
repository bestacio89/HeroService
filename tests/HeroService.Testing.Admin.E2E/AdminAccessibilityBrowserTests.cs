using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;

namespace HeroService.Testing.Admin.E2E;

public sealed class AdminAccessibilityBrowserTests : AdminBrowserTestBase
{
    [Fact]
    [Trait("Category", "E2E")]
    public Task Overview_ExposesSemanticPageStructure()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await NavigateToOverviewAsync();

            Assert.Equal(
                1,
                await Page.GetByRole(AriaRole.Main).CountAsync());

            Assert.Equal(
                1,
                await Page.GetByRole(AriaRole.Navigation, new PageGetByRoleOptions
                {
                    Name = "Primary navigation",
                    Exact = true
                }).CountAsync());

            Assert.Equal(
                1,
                await Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions
                {
                    Name = "Codex workspace",
                    Exact = true
                }).CountAsync());
        });
    }

    [Fact]
    [Trait("Category", "E2E")]
    public Task SkipLink_IsKeyboardReachableAndMovesFocusToMainContent()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await NavigateToOverviewAsync();

            await Page.Keyboard.PressAsync("Tab");

            var skipLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
            {
                Name = "Skip to main content",
                Exact = true
            });

            Assert.True(await skipLink.EvaluateAsync<bool>(
                "element => element === document.activeElement"));

            await Page.Keyboard.PressAsync("Enter");

            await Page.WaitForFunctionAsync(
                "() => document.activeElement?.id === 'main-content'");
        });
    }

    [Fact]
    [Trait("Category", "E2E")]
    public Task Overview_HasNoAutomatedWcagAOrAaViolations()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await NavigateToOverviewAsync();

            var results = await Page.RunAxe(new AxeRunOptions
            {
                RunOnly = new RunOnlyOptions
                {
                    Type = "tag",
                    Values =
                    [
                        "wcag2a",
                        "wcag2aa",
                        "wcag21a",
                        "wcag21aa",
                        "wcag22aa"
                    ]
                }
            });

            var violations = results.Violations ?? [];
            var violationSummary = string.Join(
                Environment.NewLine,
                violations.Select(violation => $"{violation.Id}: {violation.HelpUrl}"));

            Assert.True(
                !violations.Any(),
                $"axe-core reported WCAG accessibility violations:{Environment.NewLine}{violationSummary}");
        });
    }

    [Fact]
    [Trait("Category", "E2E")]
    public Task Overview_DoesNotOverflowAtMobileViewportWidth()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await Page.SetViewportSizeAsync(390, 844);
            await NavigateToOverviewAsync();

            var hasHorizontalOverflow = await Page.EvaluateAsync<bool>(
                "() => document.documentElement.scrollWidth > window.innerWidth");

            Assert.False(hasHorizontalOverflow, "The overview page overflows horizontally at a 390px viewport.");
        });
    }
}

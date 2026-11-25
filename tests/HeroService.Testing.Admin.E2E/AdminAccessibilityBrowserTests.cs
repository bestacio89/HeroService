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
                    Name = "World command center",
                    Exact = true
                }).CountAsync());

            Assert.Equal(
                1,
                await Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions
                {
                    Name = "Catalogue overview",
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

            var skipLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
            {
                Name = "Skip to main content",
                Exact = true
            });

            // Validate the real keyboard entry path: the first Tab from the page
            // must reach the first focusable element, which is the skip link.
            await Page.Keyboard.PressAsync("Tab");

            Assert.True(
                await skipLink.EvaluateAsync<bool>("element => document.activeElement === element"),
                "The first Tab from the document did not focus the skip link.");

            await Page.Keyboard.PressAsync("Enter");

            await Page.WaitForFunctionAsync(
                "() => document.getElementById('main-content') === document.activeElement",
                null,
                new PageWaitForFunctionOptions
                {
                    Timeout = 2_000
                });

            var focusEnteredMainContent = await Page.Locator("#main-content").EvaluateAsync<bool>(
                "element => element === document.activeElement");

            Assert.True(
                focusEnteredMainContent,
                "Activating the skip link did not move focus to the main content landmark.");
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
                violations.SelectMany(violation =>
                    violation.Nodes.Select(node =>
                        $"{violation.Id}: html={node.Html}; help={violation.HelpUrl}")));

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

    [Fact]
    [Trait("Category", "E2E")]
    public Task SkillCatalogue_RendersSemanticIconsAndRetainsTextLabels()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await Page.GotoAsync($"{BaseUrl.TrimEnd('/')}/admin/skills", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Load
            });

            await Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions
            {
                Name = "Skill library",
                Exact = true
            }).WaitForAsync();

            await Page.Locator("[data-testid='skill-type-icon']").First.WaitForAsync();
            Assert.True(await Page.Locator("[data-testid='skill-effect-icon']").CountAsync() > 0);
            Assert.True(await Page.GetByText("Damage", new PageGetByTextOptions { Exact = true }).CountAsync() > 0);
        });
    }

    [Fact]
    [Trait("Category", "E2E")]
    public Task HeroCatalogue_RendersClassIconsAndRetainsClassNames()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await Page.GotoAsync($"{BaseUrl.TrimEnd('/')}/admin/heroes", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.Load
            });

            await Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions
            {
                Name = "Heroes",
                Exact = true
            }).WaitForAsync();

            await Page.Locator("[data-testid='hero-class-icon']").First.WaitForAsync();
            Assert.True(await Page.GetByText("Warrior", new PageGetByTextOptions { Exact = true }).CountAsync() > 0);
        });
    }

    [Fact]
    [Trait("Category", "E2E")]
    public Task HeroAndSkillCatalogues_DoNotOverflowAtMobileViewportWidth()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await Page.SetViewportSize(390, 844);

            foreach (var route in new[] { "/admin/heroes", "/admin/skills" })
            {
                await Page.GotoAsync($"{BaseUrl.TrimEnd('/')}{route}", new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.Load
                });

                await Page.Locator("main").WaitForAsync();

                var hasHorizontalOverflow = await Page.EvaluateAsync<bool>(
                    "() => document.documentElement.scrollWidth > window.innerWidth");

                Assert.False(hasHorizontalOverflow, $"{route} overflows horizontally at a 390px viewport.");
            }
        });
    }

}

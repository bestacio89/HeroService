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

            Assert.Equal(1, await Page.GetByRole(AriaRole.Main).CountAsync());
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

            // FocusOnNavigate moves focus to the route heading after navigation.
            // Blurring that heading does not reset Chromium's sequential-focus
            // starting point. Temporarily make body a tabbable document-entry
            // anchor so the next real Tab follows DOM order from the document start.
            var focusReset = await Page.EvaluateAsync<bool>(
                "() => { document.body.setAttribute('tabindex', '0'); document.body.focus(); return document.activeElement === document.body; }");

            Assert.True(focusReset, "The browser could not focus the temporary document-entry anchor.");

            await Page.Keyboard.PressAsync("Tab");

            var focusState = await Page.EvaluateAsync<string>(
                """() => { const active = document.activeElement; const link = document.querySelector('a.skip-link'); const style = link ? getComputedStyle(link) : null; const rect = link?.getBoundingClientRect(); const firstFocusable = Array.from(document.querySelectorAll('a[href],button,input,select,textarea,[tabindex]:not([tabindex="-1"])')).slice(0, 8).map(element => element.tagName + '#' + element.id + '.' + element.className).join(','); return 'active=' + active?.tagName + '#' + active?.id + '; firstFocusable=' + firstFocusable + '; skipLink={tabIndex:' + link?.tabIndex + ',display:' + style?.display + ',visibility:' + style?.visibility + ',position:' + style?.position + ',transform:' + style?.transform + ',rect:' + (rect ? [rect.x, rect.y, rect.width, rect.height].join(',') : 'missing') + '}'; }""");

            Assert.True(
                await skipLink.EvaluateAsync<bool>("element => document.activeElement === element"),
                $"The first Tab from the document-entry anchor did not focus the skip link ({focusState}).");

            await Page.EvaluateAsync("() => document.body.removeAttribute('tabindex')");

            await Page.Keyboard.PressAsync("Enter");

            await Page.WaitForFunctionAsync(
                "() => document.getElementById('main-content') === document.activeElement",
                null,
                new PageWaitForFunctionOptions { Timeout = 2_000 });

            Assert.True(
                await Page.Locator("#main-content").EvaluateAsync<bool>("element => element === document.activeElement"),
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
    public Task SkillCatalogue_ExposesItsPageHeadingWithoutRequiringAnApiFixture()
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
        });
    }

    [Fact]
    [Trait("Category", "E2E")]
    public Task HeroCatalogue_ExposesItsPageHeadingWithoutRequiringAnApiFixture()
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
        });
    }

    [Fact]
    [Trait("Category", "E2E")]
    public Task HeroAndSkillCatalogues_DoNotOverflowAtMobileViewportWidth()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await Page.SetViewportSizeAsync(390, 844);

            foreach (var route in new[] { "/admin/heroes", "/admin/skills" })
            {
                await Page.GotoAsync($"{BaseUrl.TrimEnd('/')}{route}", new PageGotoOptions
                {
                    WaitUntil = WaitUntilState.Load
                });

                await Page.GetByRole(AriaRole.Main).WaitForAsync();

                var hasHorizontalOverflow = await Page.EvaluateAsync<bool>(
                    "() => document.documentElement.scrollWidth > window.innerWidth");

                Assert.False(hasHorizontalOverflow, $"{route} overflows horizontally at a 390px viewport.");
            }
        });
    }
}

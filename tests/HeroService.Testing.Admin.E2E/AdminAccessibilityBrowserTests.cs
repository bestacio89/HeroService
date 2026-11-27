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

            var skipLink = Page.Locator("a.skip-link");
            Assert.Equal(1, await skipLink.CountAsync());
            Assert.Equal(
                1,
                await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
                {
                    Name = "Skip to main content",
                    Exact = true
                }).CountAsync());

            // Establish a deterministic sequential-focus origin immediately
            // before the skip link. Temporarily remove other controls from the
            // tab sequence so unrelated MudBlazor controls cannot intercept Tab.
            var focusOrigin = await Page.EvaluateAsync<bool>(
                """
                () => {
                    const skipLink = document.querySelector('a.skip-link');
                    if (!skipLink) return false;

                    const focusableSelector =
                        'a[href], button:not([disabled]), input:not([disabled]), ' +
                        'select:not([disabled]), textarea:not([disabled]), ' +
                        '[tabindex]:not([tabindex="-1"])';

                    for (const element of document.querySelectorAll(focusableSelector)) {
                        if (element === skipLink) continue;

                        element.setAttribute(
                            'data-skip-link-test-original-tabindex',
                            element.hasAttribute('tabindex')
                                ? element.getAttribute('tabindex')
                                : '__missing__');
                        element.setAttribute('tabindex', '-1');
                    }

                    const origin = document.createElement('span');
                    origin.id = 'skip-link-test-focus-origin';
                    origin.tabIndex = 0;
                    origin.setAttribute('aria-label', 'Temporary keyboard focus origin');
                    skipLink.before(origin);
                    origin.focus();

                    return document.activeElement === origin;
                }
                """);

            Assert.True(focusOrigin, "The browser could not establish the temporary keyboard focus origin.");

            try
            {
                await Page.Keyboard.PressAsync("Tab");

                Assert.True(
                    await skipLink.EvaluateAsync<bool>("element => document.activeElement === element"),
                    "The skip link was not reachable by sequential keyboard navigation.");

                await Page.Keyboard.PressAsync("Enter");

                await Page.WaitForFunctionAsync(
                    "() => document.getElementById('main-content') === document.activeElement",
                    null,
                    new PageWaitForFunctionOptions { Timeout = 2_000 });

                Assert.True(
                    await Page.Locator("#main-content").EvaluateAsync<bool>("element => element === document.activeElement"),
                    "Activating the skip link did not move focus to the main content landmark.");
            }
            finally
            {
                await Page.EvaluateAsync(
                    """
                    () => {
                        document
                            .querySelectorAll('[data-skip-link-test-original-tabindex]')
                            .forEach(element => {
                                const original = element.getAttribute(
                                    'data-skip-link-test-original-tabindex');

                                if (original === '__missing__') {
                                    element.removeAttribute('tabindex');
                                } else {
                                    element.setAttribute('tabindex', original);
                                }

                                element.removeAttribute('data-skip-link-test-original-tabindex');
                            });

                        document.getElementById('skip-link-test-focus-origin')?.remove();
                    }
                    """);
            }
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

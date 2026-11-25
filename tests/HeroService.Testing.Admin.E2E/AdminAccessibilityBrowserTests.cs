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

            var navigationToggle = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions
            {
                Name = "Toggle navigation",
                Exact = true
            });

            await navigationToggle.FocusAsync();
            await Page.Keyboard.PressAsync("Shift+Tab");

            var skipLink = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
            {
                Name = "Skip to main content",
                Exact = true
            });

            Assert.True(
                await skipLink.EvaluateAsync<bool>("element => document.activeElement === element"),
                "Shift+Tab from the first navigation control did not focus the skip link.");

            await skipLink.EvaluateAsync(
                """
                element => {
                    window.__skipNavigationTrace = [];

                    const record = event => {
                        const target = event.target;

                        window.__skipNavigationTrace.push({
                            type: event.type,
                            targetTag: target instanceof HTMLElement ? target.tagName : target?.nodeName,
                            targetId: target instanceof HTMLElement ? target.id : null,
                            targetClass: target instanceof HTMLElement ? target.className : null,
                            defaultPrevented: event.defaultPrevented,
                            hash: window.location.hash
                        });
                    };

                    element.addEventListener("click", record);
                    document.addEventListener("focusin", record);
                    document.addEventListener("focusout", record);
                    window.addEventListener("hashchange", record);
                }
                """);

            await Page.Keyboard.PressAsync("Enter");

            await Page.WaitForFunctionAsync(
                "() => window.__skipNavigationTrace?.some(event => event.type === 'click')",
                null,
                new PageWaitForFunctionOptions
                {
                    Timeout = 2_000
                });

            var navigationState = await Page.EvaluateAsync<string>(
                """
                () => JSON.stringify({
                    url: window.location.href,
                    hash: window.location.hash,
                    activeElement: document.activeElement instanceof HTMLElement
                        ? {
                            tagName: document.activeElement.tagName,
                            id: document.activeElement.id,
                            className: document.activeElement.className,
                            text: document.activeElement.textContent?.trim()
                        }
                        : null,
                    mainContent: (() => {
                        const element = document.getElementById("main-content");

                        return element instanceof HTMLElement
                            ? {
                                tagName: element.tagName,
                                tabIndex: element.tabIndex,
                                containsActiveElement: element.contains(document.activeElement),
                                outerHtml: element.outerHTML.slice(0, 500)
                            }
                            : null;
                    })(),
                    trace: window.__skipNavigationTrace
                })
                """);

            var focusEnteredMainContent = await Page.Locator("#main-content").EvaluateAsync<bool>(
                "element => element.contains(document.activeElement)");

            Assert.True(
                focusEnteredMainContent,
                $"Activating the skip link did not move focus into the main content region. Browser state: {navigationState}");
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
}

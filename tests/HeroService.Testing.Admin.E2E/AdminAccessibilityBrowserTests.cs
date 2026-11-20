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

            var skipLinkCount = await Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions
            {
                Name = "Skip to main content",
                Exact = true
            }).CountAsync();

            Assert.Equal(1, skipLinkCount);

            await Page.Keyboard.PressAsync("Tab");

            var skipLinkHasFocus = await Page.EvaluateAsync<bool>(
                "document.activeElement instanceof HTMLAnchorElement && document.activeElement.classList.contains('skip-link')");

            if (!skipLinkHasFocus)
            {
                var focusDiagnostics = await CaptureFocusDiagnosticsAsync();

                Assert.Fail(
                    $"The first Tab did not focus the skip link. Browser focus state: {focusDiagnostics}");
            }

            await Page.EvaluateAsync(
                """
                () => {
                    window.__heroServiceFocusTrace = [];
                    document.addEventListener("focusin", event => {
                        const element = event.target;
                        window.__heroServiceFocusTrace.push({
                            event: "focusin",
                            tag: element?.tagName ?? null,
                            id: element?.id ?? null,
                            className: typeof element?.className === "string" ? element.className : ""
                        });
                    }, true);
                    document.addEventListener("focusout", event => {
                        const element = event.target;
                        window.__heroServiceFocusTrace.push({
                            event: "focusout",
                            tag: element?.tagName ?? null,
                            id: element?.id ?? null,
                            className: typeof element?.className === "string" ? element.className : ""
                        });
                    }, true);
                }
                """);

            await Page.Keyboard.PressAsync("Enter");

            try
            {
                await Page.WaitForFunctionAsync(
                    "() => document.activeElement?.id === 'main-content'",
                    new FrameWaitForFunctionOptions
                    {
                        Timeout = 2_000
                    });
            }
            catch (TimeoutException)
            {
                var focusDiagnostics = await CaptureFocusDiagnosticsAsync();

                Assert.Fail(
                    $"Activating the skip link did not focus #main-content. Browser focus state: {focusDiagnostics}");
            }

            var targetHasFocus = await Page.EvaluateAsync<bool>(
                """
                () => {
                    const target = document.getElementById("main-content");
                    return target !== null && document.activeElement === target;
                }
                """);

            Assert.True(
                targetHasFocus,
                $"The active element was not the main content target. Browser focus state: {await CaptureFocusDiagnosticsAsync()}");
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

    private async Task<string> CaptureFocusDiagnosticsAsync()
    {
        return await Page.EvaluateAsync<string>(
            """
            () => {
                const target = document.getElementById("main-content");
                const active = document.activeElement;
                const skipLink = document.querySelector("a.skip-link");

                return JSON.stringify({
                    url: window.location.href,
                    hash: window.location.hash,
                    target: target ? {
                        tag: target.tagName,
                        id: target.id,
                        className: typeof target.className === "string" ? target.className : "",
                        role: target.getAttribute("role"),
                        tabIndex: target.getAttribute("tabindex"),
                        html: target.outerHTML.slice(0, 500)
                    } : null,
                    activeElement: active ? {
                        tag: active.tagName,
                        id: active.id,
                        className: typeof active.className === "string" ? active.className : "",
                        role: active.getAttribute("role"),
                        tabIndex: active.getAttribute("tabindex"),
                        html: active.outerHTML?.slice(0, 500) ?? null
                    } : null,
                    activeIsTarget: target !== null && active === target,
                    activeInsideTarget: target !== null && active !== null && target.contains(active),
                    skipLink: skipLink ? {
                        href: skipLink.getAttribute("href"),
                        className: skipLink.className
                    } : null,
                    focusTrace: window.__heroServiceFocusTrace ?? []
                });
            }
            """);
    }
}

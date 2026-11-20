using System.Runtime.CompilerServices;
using Microsoft.Playwright;

namespace HeroService.Testing.Admin.E2E;

public abstract class AdminBrowserTestBase : IAsyncLifetime
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    protected string BaseUrl { get; } =
        Environment.GetEnvironmentVariable("ADMIN_BASE_URL")
        ?? throw new InvalidOperationException("ADMIN_BASE_URL must be configured for browser tests.");

    protected IPage Page { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        Page = await _browser.NewPageAsync(new BrowserNewPageOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = 1440,
                Height = 1000
            }
        });
    }

    public async Task DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }

    protected async Task NavigateToOverviewAsync()
    {
        var response = await Page.GotoAsync(BaseUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.Load
        });

        Assert.NotNull(response);
        Assert.True(
            response.Ok,
            $"The admin application returned HTTP {response.Status} for {BaseUrl}.");

        await Page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions
        {
            Name = "Codex workspace",
            Exact = true
        }).WaitForAsync();
    }

    protected async Task RunWithDiagnosticsAsync(
        Func<Task> test,
        [CallerMemberName] string testName = "")
    {
        try
        {
            await test();
        }
        catch
        {
            var diagnosticsDirectory = Environment.GetEnvironmentVariable("ADMIN_DIAGNOSTICS_DIRECTORY");

            if (!string.IsNullOrWhiteSpace(diagnosticsDirectory))
            {
                try
                {
                    Directory.CreateDirectory(diagnosticsDirectory);
                    await Page.ScreenshotAsync(new PageScreenshotOptions
                    {
                        Path = Path.Combine(diagnosticsDirectory, $"{SanitizeFileName(testName)}.png"),
                        FullPage = true
                    });
                }
                catch
                {
                    // Preserve the original browser-test failure if diagnostics cannot be captured.
                }
            }

            throw;
        }
    }

    private static string SanitizeFileName(string value)
    {
        var invalidCharacters = Path.GetInvalidFileNameChars();
        var sanitized = new string(value.Select(character =>
            invalidCharacters.Contains(character) ? '_' : character).ToArray());

        return string.IsNullOrWhiteSpace(sanitized) ? "browser-test-failure" : sanitized;
    }
}

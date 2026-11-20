using System.Text.Json;
using Microsoft.Playwright;

namespace HeroService.Testing.Admin.E2E;

public sealed class AdminPerformanceBrowserTests : AdminBrowserTestBase
{
    [Fact]
    [Trait("Category", "E2E")]
    public Task Overview_ProducesBrowserPerformanceMeasurements()
    {
        return RunWithDiagnosticsAsync(async () =>
        {
            await Page.AddInitScriptAsync(
                """
                (() => {
                    window.__heroServiceLargestContentfulPaint = 0;

                    new PerformanceObserver(entries => {
                        const latestEntry = entries.getEntries().at(-1);

                        if (latestEntry) {
                            window.__heroServiceLargestContentfulPaint = latestEntry.startTime;
                        }
                    }).observe({ type: "largest-contentful-paint", buffered: true });
                })();
                """);

            await NavigateToOverviewAsync();

            await Page.WaitForFunctionAsync(
                "() => window.__heroServiceLargestContentfulPaint > 0",
                null,
                new PageWaitForFunctionOptions
                {
                    Timeout = 5000
                });

            var navigationMetrics = await Page.EvaluateAsync<Dictionary<string, double>>(
                """
                () => {
                    const navigation = performance.getEntriesByType("navigation")[0];
                    const firstContentfulPaint = performance.getEntriesByName("first-contentful-paint")[0];

                    return {
                        domContentLoadedMs: navigation?.domContentLoadedEventEnd ?? 0,
                        loadEventMs: navigation?.loadEventEnd ?? 0,
                        firstContentfulPaintMs: firstContentfulPaint?.startTime ?? 0,
                        largestContentfulPaintMs: window.__heroServiceLargestContentfulPaint ?? 0,
                        transferSizeBytes: navigation?.transferSize ?? 0
                    };
                }
                """);

            var metrics = new AdminBrowserPerformanceMetrics(
                DateTimeOffset.UtcNow,
                Page.Url,
                navigationMetrics["domContentLoadedMs"],
                navigationMetrics["loadEventMs"],
                navigationMetrics["firstContentfulPaintMs"],
                navigationMetrics["largestContentfulPaintMs"],
                navigationMetrics["transferSizeBytes"]);

            Assert.True(metrics.DomContentLoadedMs > 0);
            Assert.True(metrics.FirstContentfulPaintMs > 0);
            Assert.True(metrics.LargestContentfulPaintMs > 0);

            var reportPath = Environment.GetEnvironmentVariable("ADMIN_PERFORMANCE_RESULTS_PATH");

            if (!string.IsNullOrWhiteSpace(reportPath))
            {
                var reportDirectory = Path.GetDirectoryName(reportPath);

                if (!string.IsNullOrWhiteSpace(reportDirectory))
                {
                    Directory.CreateDirectory(reportDirectory);
                }

                await File.WriteAllTextAsync(
                    reportPath,
                    JsonSerializer.Serialize(metrics, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
            }
        });
    }
}

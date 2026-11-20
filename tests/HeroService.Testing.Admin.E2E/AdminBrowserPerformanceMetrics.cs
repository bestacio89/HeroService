namespace HeroService.Testing.Admin.E2E;

public sealed record AdminBrowserPerformanceMetrics(
    DateTimeOffset MeasuredAtUtc,
    string Url,
    double DomContentLoadedMs,
    double LoadEventMs,
    double FirstContentfulPaintMs,
    double LargestContentfulPaintMs,
    double TransferSizeBytes);

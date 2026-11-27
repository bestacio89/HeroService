namespace HeroService.Admin.Operational;

public sealed record OperationalCheckResult(
    string Service,
    string Check,
    OperationalCheckStatus Status,
    DateTimeOffset? LastCheckedAt,
    TimeSpan? Duration,
    string Message);

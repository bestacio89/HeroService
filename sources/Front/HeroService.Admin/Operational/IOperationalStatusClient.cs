namespace HeroService.Admin.Operational;

public interface IOperationalStatusClient
{
    Task<IReadOnlyList<OperationalCheckResult>> GetStatusesAsync(
        CancellationToken cancellationToken = default);
}

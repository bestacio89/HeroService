using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HeroService.Admin.Operational;

public sealed partial class OperationalVisibility : ComponentBase, IDisposable
{
    [Inject]
    private IOperationalStatusClient StatusClient { get; set; } = default!;

    [Inject]
    private ILogger<OperationalVisibility> Logger { get; set; } = default!;

    private readonly CancellationTokenSource _lifetime = new();
    private IReadOnlyList<OperationalCheckResult> _checks = [];
    private bool _isLoading;
    private bool _disposed;

    protected override Task OnInitializedAsync() => RefreshAsync();

    private async Task RefreshAsync()
    {
        if (_isLoading || _disposed)
        {
            return;
        }

        _isLoading = true;

        try
        {
            _checks = await StatusClient.GetStatusesAsync(_lifetime.Token);
        }
        catch (OperationCanceledException) when (_lifetime.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            Logger.LogError(exception, "Unable to retrieve operational visibility status.");
            _checks = [];
        }
        finally
        {
            if (!_disposed)
            {
                _isLoading = false;
            }
        }
    }

    private static Color GetStatusColor(OperationalCheckStatus status) =>
        status switch
        {
            OperationalCheckStatus.Healthy => Color.Success,
            OperationalCheckStatus.Degraded => Color.Warning,
            OperationalCheckStatus.Unhealthy => Color.Error,
            OperationalCheckStatus.Unavailable => Color.Error,
            _ => Color.Default
        };

    private static string FormatStatus(OperationalCheckStatus status) =>
        status switch
        {
            OperationalCheckStatus.NotChecked => "Not checked",
            _ => status.ToString()
        };

    private static string FormatCheckedAt(DateTimeOffset? value) =>
        value?.ToLocalTime().ToString("HH:mm:ss") ?? "Not checked";

    private static string FormatDuration(TimeSpan? value) =>
        value is null ? "—" : $"{value.Value.TotalMilliseconds:N0} ms";

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _lifetime.Cancel();
        _lifetime.Dispose();
    }
}

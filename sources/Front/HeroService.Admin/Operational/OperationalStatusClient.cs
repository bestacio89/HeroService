using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HeroService.Admin.Operational;

public sealed class OperationalStatusClient : IOperationalStatusClient
{
    public const string ApiClientName = "OperationalVisibility.Api";
    public const string ConsumerClientName = "OperationalVisibility.Consumer";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OperationalStatusClient> _logger;

    public OperationalStatusClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OperationalStatusClient> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<OperationalCheckResult>> GetStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        var apiChecks = CheckServiceAsync(
            "HeroService API",
            ApiClientName,
            "/health/live",
            "/health/ready",
            cancellationToken);

        var consumerBaseUrl = _configuration["OperationalVisibility:ConsumerBaseUrl"];

        if (string.IsNullOrWhiteSpace(consumerBaseUrl))
        {
            var apiResults = await apiChecks;
            return
            [
                .. apiResults,
                NotConfigured("HeroService Consumer", "Liveness"),
                NotConfigured("HeroService Consumer", "Readiness")
            ];
        }

        var consumerChecks = CheckServiceAsync(
            "HeroService Consumer",
            ConsumerClientName,
            "/health/live",
            "/health/ready",
            cancellationToken);

        var results = await Task.WhenAll(apiChecks, consumerChecks);
        return results.SelectMany(result => result).ToArray();
    }

    private async Task<IReadOnlyList<OperationalCheckResult>> CheckServiceAsync(
        string service,
        string clientName,
        string livenessPath,
        string readinessPath,
        CancellationToken cancellationToken)
    {
        var liveness = CheckEndpointAsync(service, "Liveness", clientName, livenessPath, cancellationToken);
        var readiness = CheckEndpointAsync(service, "Readiness", clientName, readinessPath, cancellationToken);

        return await Task.WhenAll(liveness, readiness);
    }

    private async Task<OperationalCheckResult> CheckEndpointAsync(
        string service,
        string check,
        string clientName,
        string path,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var checkedAt = DateTimeOffset.UtcNow;

        try
        {
            var client = _httpClientFactory.CreateClient(clientName);
            using var response = await client.GetAsync(
                path,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var status = ParseStatus(body);

            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                status = OperationalCheckStatus.Unhealthy;
            }
            else if (!response.IsSuccessStatusCode && status is not OperationalCheckStatus.Unhealthy)
            {
                status = OperationalCheckStatus.Unavailable;
            }

            return CreateResult(service, check, status, checkedAt, stopwatch.Elapsed);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException exception)
        {
            _logger.LogWarning(exception, "Operational {Check} request timed out for {Service}.", check, service);
            return CreateResult(service, check, OperationalCheckStatus.Unavailable, checkedAt, stopwatch.Elapsed);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Operational {Check} endpoint is unavailable for {Service}.", check, service);
            return CreateResult(service, check, OperationalCheckStatus.Unavailable, checkedAt, stopwatch.Elapsed);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected failure while checking {Check} for {Service}.", check, service);
            return CreateResult(service, check, OperationalCheckStatus.Unavailable, checkedAt, stopwatch.Elapsed);
        }
    }

    private static OperationalCheckStatus ParseStatus(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return OperationalCheckStatus.Unavailable;
        }

        var value = content.Trim();

        if (value.StartsWith('{'))
        {
            try
            {
                using var document = JsonDocument.Parse(value);
                if (document.RootElement.TryGetProperty("status", out var statusElement))
                {
                    value = statusElement.GetString() ?? string.Empty;
                }
            }
            catch (JsonException)
            {
                return OperationalCheckStatus.Unavailable;
            }
        }

        return value.ToUpperInvariant() switch
        {
            "HEALTHY" => OperationalCheckStatus.Healthy,
            "DEGRADED" => OperationalCheckStatus.Degraded,
            "UNHEALTHY" => OperationalCheckStatus.Unhealthy,
            _ => OperationalCheckStatus.Unavailable
        };
    }

    private static OperationalCheckResult NotConfigured(string service, string check) =>
        new(
            service,
            check,
            OperationalCheckStatus.NotChecked,
            null,
            null,
            "The Consumer endpoint is not configured.");

    private static OperationalCheckResult CreateResult(
        string service,
        string check,
        OperationalCheckStatus status,
        DateTimeOffset checkedAt,
        TimeSpan duration)
    {
        var message = status switch
        {
            OperationalCheckStatus.Healthy => "The endpoint reported healthy.",
            OperationalCheckStatus.Degraded => "The endpoint reported degraded service.",
            OperationalCheckStatus.Unhealthy => "The endpoint reported an unhealthy state.",
            _ => "The endpoint could not be reached or returned an unsupported response."
        };

        return new OperationalCheckResult(service, check, status, checkedAt, duration, message);
    }
}

using System.Net;
using HeroService.Admin.Operational;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;

namespace HeroService.Testing.Admin;

public sealed class OperationalStatusClientTests
{
    [Fact]
    public async Task GetStatusesAsync_MapsIndependentServiceAndEndpointStates()
    {
        var factory = new TestHttpClientFactory(new Dictionary<string, HttpClient>
        {
            [OperationalStatusClient.ApiClientName] = CreateClient((request, _) =>
                request.RequestUri!.AbsolutePath.EndsWith("/live", StringComparison.Ordinal)
                    ? JsonResponse(HttpStatusCode.OK, """{"status":"Healthy"}""")
                    : JsonResponse(HttpStatusCode.ServiceUnavailable, """{"status":"Unhealthy"}""")),
            [OperationalStatusClient.ConsumerClientName] = CreateClient((request, _) =>
                request.RequestUri!.AbsolutePath.EndsWith("/live", StringComparison.Ordinal)
                    ? JsonResponse(HttpStatusCode.OK, "Healthy")
                    : JsonResponse(HttpStatusCode.OK, "Degraded"))
        });

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OperationalVisibility:ConsumerBaseUrl"] = "https://consumer.example.test"
            })
            .Build();

        var client = new OperationalStatusClient(
            factory,
            configuration,
            NullLogger<OperationalStatusClient>.Instance);

        var results = await client.GetStatusesAsync();

        Assert.Equal(4, results.Count);
        Assert.Equal(OperationalCheckStatus.Healthy, Find(results, "HeroService API", "Liveness").Status);
        Assert.Equal(OperationalCheckStatus.Unhealthy, Find(results, "HeroService API", "Readiness").Status);
        Assert.Equal(OperationalCheckStatus.Healthy, Find(results, "HeroService Consumer", "Liveness").Status);
        Assert.Equal(OperationalCheckStatus.Degraded, Find(results, "HeroService Consumer", "Readiness").Status);
        Assert.All(results, result => Assert.NotNull(result.LastCheckedAt));
        Assert.All(results, result => Assert.NotNull(result.Duration));
    }

    [Fact]
    public async Task GetStatusesAsync_LeavesConsumerNotCheckedWhenEndpointIsNotConfigured()
    {
        var factory = new TestHttpClientFactory(new Dictionary<string, HttpClient>
        {
            [OperationalStatusClient.ApiClientName] = CreateClient((_, _) =>
                JsonResponse(HttpStatusCode.OK, """{"status":"Healthy"}""")),
            [OperationalStatusClient.ConsumerClientName] = CreateClient((_, _) =>
                throw new InvalidOperationException("Consumer should not be queried."))
        });

        var configuration = new ConfigurationBuilder().Build();
        var client = new OperationalStatusClient(
            factory,
            configuration,
            NullLogger<OperationalStatusClient>.Instance);

        var results = await client.GetStatusesAsync();

        Assert.Equal(4, results.Count);
        Assert.All(
            results.Where(result => result.Service == "HeroService Consumer"),
            result => Assert.Equal(OperationalCheckStatus.NotChecked, result.Status));
        Assert.All(
            results.Where(result => result.Service == "HeroService API"),
            result => Assert.Equal(OperationalCheckStatus.Healthy, result.Status));
    }

    private static OperationalCheckResult Find(
        IReadOnlyList<OperationalCheckResult> results,
        string service,
        string check) =>
        Assert.Single(results.Where(result => result.Service == service && result.Check == check));

    private static HttpClient CreateClient(
        Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> responseFactory)
    {
        var client = new HttpClient(new DelegateHttpMessageHandler(responseFactory))
        {
            BaseAddress = new Uri("https://api.example.test"),
            Timeout = TimeSpan.FromSeconds(2)
        };

        return client;
    }

    private static HttpResponseMessage JsonResponse(HttpStatusCode statusCode, string content) =>
        new(statusCode)
        {
            Content = new StringContent(content)
        };

    private sealed class TestHttpClientFactory(IReadOnlyDictionary<string, HttpClient> clients)
        : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => clients[name];
    }

    private sealed class DelegateHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> responseFactory)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(responseFactory(request, cancellationToken));
    }
}

using Franz.Common.Messaging.Bootstrap.Extenstions;
using HeroService.Consumer.Extensions;
using HeroService.Consumer.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HeroService.Consumer;

public static class Program
{
  public static async Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Configuration
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
      .AddJsonFile(
          $"appsettings.{builder.Environment.EnvironmentName.ToLowerInvariant()}.json",
          optional: true,
          reloadOnChange: true)
      .AddEnvironmentVariables();

    builder.WebHost.UseUrls(
        builder.Configuration["ASPNETCORE_URLS"] ?? "http://0.0.0.0:8080");

    var configuration = builder.Configuration;
    var environment = builder.Environment;

    builder.Services.AddHeroServiceConsumerPersistence(configuration, environment);
    builder.Services.AddMessagingArchitecture(environment, configuration);
    builder.Services.AddKafkaMessagingServices(configuration, environment.EnvironmentName);
    builder.Services.AddHealthChecks()
        .AddCheck<KafkaBrokerHealthCheck>("kafka", tags: new[] { "ready" });

    using var app = builder.Build();

    // Liveness deliberately excludes external dependencies so transient broker outages
    // do not cause restart storms. Readiness checks broker metadata and required topics.
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
      Predicate = _ => false
    });
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
      Predicate = check => check.Tags.Contains("ready")
    });

    await app.RunAsync();
  }
}

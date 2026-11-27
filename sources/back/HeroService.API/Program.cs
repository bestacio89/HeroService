using System.Text.Json;
using Franz.Common.Business.Extensions;
using Franz.Common.Caching.Abstractions;
using Franz.Common.Caching.Extensions;
using HeroService.API.Caching;
using HeroService.API.Extensions;
using HeroService.API.HealthChecks;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.EntityFramework.Extensions;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Logging.Extensions;
using Franz.Common.Mediator.Bootstrap;
using Franz.Common.Mediator.Extensions;
using Franz.Common.Mediator.Polly;
using HeroService.Application;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence;
using HeroService.Persistence.Persistence.Seeding;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
var config = builder.Configuration;

// =========================================================
// LOGGING
// =========================================================
builder.Host.UseLog();

// =========================================================
// APPLICATION
// =========================================================
builder.Services.RegisterApplicationServices();

// =========================================================
// BUSINESS (Domain + Mediator + Handlers)
// =========================================================
builder.Services.AddBusiness(typeof(SkillEffect).Assembly);

// =========================================================
// DATABASE
// =========================================================
builder.Services.AddRelationalDatabase<ApplicationDbContext>(env, config);
builder.Services.AddFranzAuditing();

// =========================================================
// PERSISTENCE
// =========================================================
builder.Services.RegisterPersistenceServices<ApplicationDbContext>(config);

// =========================================================
// HTTP
// =========================================================
builder.Services.AddHeroServiceHttpArchitecture(env, config);

// =========================================================
// MEDIATOR PIPELINES
// =========================================================
builder.Services.AddFranzMediatorV2Default(
        null,
        typeof(CreateHeroCommandHandler).Assembly)
    .AddFranzSerilogAuditPipeline()
    .AddFranzSerilogLoggingPipeline();

builder.Services.AddFranzMediatorCaching(config);
builder.Services.AddFranzCachingJsonTypeInfoResolver(HeroServiceJsonSerializerContext.Default);
builder.Services.AddSingleton<ICacheMetadataProvider, HeroServiceQueryCacheMetadataProvider>();

builder.Services.AddFranzResilience(config);

// =========================================================
// OPERATIONAL HEALTH
// =========================================================
builder.Services
    .AddHealthChecks()
    .AddCheck<DatabaseReadinessHealthCheck>(
        "postgresql",
        tags: ["ready"]);

// =========================================================
// BUILD
// =========================================================
var app = builder.Build();

// =========================================================
// DATABASE INITIALIZATION
// =========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var cancellationToken = CancellationToken.None;

    if (app.Environment.IsDevelopment())
    {
        var database = services.GetRequiredService<ApplicationDbContext>();
        var seeder = services.GetRequiredService<DatabaseSeeder>();

        await database.Database.MigrateAsync();
        await seeder.RunAsync(cancellationToken);
    }
    else
    {
        Log.Information(
            "Higher Environment detected. Database runtime initialization handled by orchestration plane.");
    }
}

await HeroServiceCacheWarmup.WarmAsync(
    app.Services,
    app.Lifetime.ApplicationStopping);

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

// Franz.Common.UseHttpArchitecture maps controller endpoints and applies
// the configured default authorization policy. Do not map controllers again.
app.UseHttpArchitecture();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = WriteHealthResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = WriteHealthResponse
});

app.Run();

static async Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var payload = new
    {
        status = report.Status.ToString(),
        durationMs = report.TotalDuration.TotalMilliseconds,
        checks = report.Entries.Select(entry => new
        {
            name = entry.Key,
            status = entry.Value.Status.ToString(),
            durationMs = entry.Value.Duration.TotalMilliseconds
        })
    };

    await JsonSerializer.SerializeAsync(
        context.Response.Body,
        payload,
        cancellationToken: context.RequestAborted);
}

public partial class Program { }

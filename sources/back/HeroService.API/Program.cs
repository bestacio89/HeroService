using HeroService.Application;
using HeroService.Persistence;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Logging.Extensions;
using Franz.Common.Mediator.Extensions;
using Franz.Common.Mediator.Polly;
using Microsoft.EntityFrameworkCore;
using Serilog;
using HeroService.Persistence.Persistence.Seeding;
using Franz.Common.Mediator.Bootstrap;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var config = builder.Configuration;

// =========================================================
// LOGGING SUBSYSTEM
// =========================================================
builder.Host.UseLog();

// =========================================================
// APPLICATION & PERSISTENCE SERVICES
// =========================================================
builder.Services.RegisterApplicationServices();

// Standardized Relational Database Registration
builder.Services.AddRelationalDatabase<ApplicationDbContext>(env, config);

// =========================================================
// HTTP ARCHITECTURE & DOCUMENTATION
// =========================================================
builder.Services.AddHttpArchitecture(env, config);

// =========================================================
// MEDIATOR & RESILIENCE PIPELINES
// =========================================================
builder.Services.AddFranzMediatorStandard(new[] { typeof(CreateHeroCommandHandler).Assembly });
builder.Services.AddFranzResilience(config);

var app = builder.Build();

// =========================================================
// LIFECYCLE & ENVIRONMENT BOUNDARY ENVIRONMENT STATES
// =========================================================
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;

  if (app.Environment.IsDevelopment())
  {
    // Local Dev State: Safely execute incremental migrations and apply idempotent seeding
    var db = services.GetRequiredService<ApplicationDbContext>();
    var seeder = services.GetRequiredService<DatabaseSeeder>();

    await db.Database.MigrateAsync(CancellationToken.None);
    await seeder.RunAsync(CancellationToken.None);
  }
  else
  {
    // Hardened Production State: Runtime instance has ZERO DDL/Migration access.
    // Schema generation is shifted entirely left to the CI/CD deployment phase.
    Log.Information("Higher Environment detected. Database runtime initialization handled by orchestration plane.");
  }
}

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

// =========================================================
// MIDDLEWARE PIPELINE
// =========================================================
if (app.Environment.IsDevelopment())
{
  app.UseDocumentation();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
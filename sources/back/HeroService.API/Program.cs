using Franz.Common.Business.Extensions;
using Franz.Common.EntityFramework.Auditing;
using Franz.Common.EntityFramework.Extensions;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Logging.Extensions;
using Franz.Common.Mediator.Bootstrap;
using Franz.Common.Mediator.Polly;
using HeroService.Application;
using HeroService.Domain.Heroes.Skills;
using HeroService.Persistence;
using HeroService.Persistence.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
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
// Add this after AddRelationalDatabase
builder.Services.AddFranzAuditing();

// =========================================================
// PERSISTENCE
// =========================================================
builder.Services.RegisterPersistenceServices<ApplicationDbContext>(config);

// =========================================================
// HTTP
// =========================================================
builder.Services.AddHttpArchitecture(env, config);

// =========================================================
// MEDIATOR PIPELINES
// =========================================================
builder.Services.AddFranzMediatorStandard(
    new[] { typeof(CreateHeroCommandHandler).Assembly });

builder.Services.AddFranzResilience(config);

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
  var ct = CancellationToken.None;
  if (app.Environment.IsDevelopment())
  {
    var db = services.GetRequiredService<ApplicationDbContext>();
    var seeder = services.GetRequiredService<DatabaseSeeder>();

    await db.Database.MigrateAsync();
    await seeder.RunAsync(ct);
  }
  else
  {
    Log.Information(
        "Higher Environment detected. Database runtime initialization handled by orchestration plane.");
  }
}

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

// =========================================================
// MIDDLEWARE
// =========================================================
app.UseHttpArchitecture();
//app.UseAuthorization();

app.MapControllers();

app.Run();
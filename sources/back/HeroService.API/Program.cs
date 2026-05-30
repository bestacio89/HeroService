
using HeroService.Application;
using HeroService.Persistence;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.EntityFramework.Extensions;
using Franz.Common.Http.Documentation.Extensions;
using Franz.Common.Logging.Extensions;
using Franz.Common.Mediator.Extensions;
using Franz.Common.Mediator.Polly;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using HeroService.Persistence.Persistence.Seeding;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var config = builder.Configuration;

// --- Logging ---
builder.Host.UseLog();
builder.Services.AddFranzSerilogAuditPipeline()
                .AddFranzEventValidationPipeline()
                .AddFranzSerilogLoggingPipeline()
                .AddFranzTelemetry(env, config);

// --- Application & Persistence ---
builder.Services.RegisterApplicationServices();
// Standardized Database registration - handles DbContext and Persistence Services
builder.Services.AddRelationalDatabase<ApplicationDbContext>(env, config);

// --- Http Architecture & Documentation ---
// This handles Controllers, Versioning, and Swagger via your internal logic
builder.Services.AddHttpArchitecture(env, config);

// --- Mediator + Pipelines ---
builder.Services.AddFranzMediator(new[] { typeof(CreateHeroCommandHandler).Assembly });
builder.Services.AddFranzResilience(config);


var app = builder.Build();

// --- DB Initialization ---
using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
  var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
  if (app.Environment.IsDevelopment())
  {
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
  }
  else
  {
    db.Database.Migrate();
  }
  await seeder.RunAsync(CancellationToken.None);
}

app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

// --- Middleware ---
if (app.Environment.IsDevelopment())
{
  // Use the framework-level documentation helper
  app.UseDocumentation();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
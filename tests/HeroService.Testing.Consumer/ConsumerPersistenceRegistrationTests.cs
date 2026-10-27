using HeroService.Consumer.Extensions;
using HeroService.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace HeroService.Testing.Consumer;

public sealed class ConsumerPersistenceRegistrationTests : IAsyncLifetime
{
  private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16.10-alpine")
    .WithDatabase("hero_service_consumer_tests")
    .WithUsername("postgres")
    .WithPassword("postgres")
    .Build();

  private readonly RedisContainer _redis = new RedisBuilder("redis:7.4.2-alpine").Build();
  private ServiceProvider? _services;

  public async Task InitializeAsync()
  {
    await _postgres.StartAsync();
    await _redis.StartAsync();

    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["Database:Provider"] = "Postgres",
        ["Database:ServerName"] = _postgres.Hostname,
        ["Database:DatabaseName"] = "hero_service_consumer_tests",
        ["Database:UserName"] = "postgres",
        ["Database:Password"] = "postgres",
        ["Database:Port"] = _postgres.GetMappedPublicPort(5432).ToString(),
        ["Database:Ssl"] = "Disable",
        ["Caching:DefaultAbsoluteExpiration"] = "00:05:00",
        ["Caching:DefaultSlidingExpiration"] = "00:02:00",
        ["Caching:KeyPrefix"] = "HeroServiceConsumerTests",
        ["Caching:ConnectionString"] = _redis.GetConnectionString()
      })
      .Build();

    var services = new ServiceCollection();
    services.AddLogging();
    services.AddHttpContextAccessor();
    services.AddHeroServiceConsumerPersistence(configuration, new TestHostEnvironment());
    _services = services.BuildServiceProvider();
  }

  public async Task DisposeAsync()
  {
    if (_services is not null)
      await _services.DisposeAsync();

    await _redis.DisposeAsync();
    await _postgres.DisposeAsync();
  }

  [Fact]
  [Trait("Category", "Integration")]
  public async Task ConsumerPersistenceRegistration_UsesPostgresAndAppliesMigrations()
  {
    await using var scope = _services!.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", db.Database.ProviderName);

    await db.Database.MigrateAsync();

    Assert.True(await db.Database.CanConnectAsync());
  }

  private sealed class TestHostEnvironment : IHostEnvironment
  {
    public string EnvironmentName { get; set; } = "Testing";
    public string ApplicationName { get; set; } = "HeroService.Consumer";
    public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
  }
}

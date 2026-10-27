using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HeroService.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace HeroService.Testing.API.Integration;

public sealed class HeroClassApiIntegrationTests : IAsyncLifetime
{
  private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16.10-alpine")
    .WithDatabase("hero_service_api_tests")
    .WithUsername("postgres")
    .WithPassword("postgres")
    .Build();

  private readonly RedisContainer _redis = new RedisBuilder("redis:7.4.2-alpine").Build();
  private HeroServiceApiFactory? _factory;
  private HttpClient? _client;

  public async Task InitializeAsync()
  {
    await _postgres.StartAsync();
    await _redis.StartAsync();

    _factory = new HeroServiceApiFactory(_postgres, _redis);
    _client = _factory.CreateClient();

    await using var scope = _factory.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
  }

  public async Task DisposeAsync()
  {
    _client?.Dispose();

    if (_factory is not null)
      await _factory.DisposeAsync();

    await _redis.DisposeAsync();
    await _postgres.DisposeAsync();
  }

  [Fact]
  [Trait("Category", "Integration")]
  public async Task GetHeroClasses_ReturnsOkWithJsonArray()
  {
    using var response = await _client!.GetAsync("/api/v1/hero-classes");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

    var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
    Assert.Equal(JsonValueKind.Array, payload.ValueKind);
  }

  [Fact]
  [Trait("Category", "Integration")]
  public async Task GetHeroClassById_ReturnsNotFoundWhenMissing()
  {
    using var response = await _client!.GetAsync($"/api/v1/hero-classes/{Guid.NewGuid()}");

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  private sealed class HeroServiceApiFactory : WebApplicationFactory<Program>
  {
    private readonly PostgreSqlContainer _database;
    private readonly RedisContainer _cache;

    public HeroServiceApiFactory(PostgreSqlContainer database, RedisContainer cache)
    {
      _database = database;
      _cache = cache;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.UseEnvironment("Production");
      builder.ConfigureAppConfiguration((_, configuration) =>
      {
        var values = new Dictionary<string, string?>
        {
          ["Database:Provider"] = "Postgres",
          ["Database:ServerName"] = _database.Hostname,
          ["Database:DatabaseName"] = "hero_service_api_tests",
          ["Database:UserName"] = "postgres",
          ["Database:Password"] = "postgres",
          ["Database:Port"] = _database.GetMappedPublicPort(5432).ToString(),
          ["Database:Ssl"] = "Disable",
          ["Caching:ConnectionString"] = _cache.GetConnectionString(),
          ["Caching:KeyPrefix"] = "HeroServiceTests"
        };

        configuration.AddInMemoryCollection(values);
      });
    }
  }
}

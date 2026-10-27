using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HeroService.Persistence;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
      builder.ConfigureTestServices(services =>
      {
        services.AddAuthentication(options =>
        {
          options.DefaultAuthenticateScheme = "IntegrationTest";
          options.DefaultChallengeScheme = "IntegrationTest";
          options.DefaultScheme = "IntegrationTest";
        }).AddScheme<AuthenticationSchemeOptions, IntegrationTestAuthenticationHandler>(
          "IntegrationTest", _ => { });
      });
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

  private sealed class IntegrationTestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
  {
    public IntegrationTestAuthenticationHandler(
      IOptionsMonitor<AuthenticationSchemeOptions> options,
      ILoggerFactory logger,
      UrlEncoder encoder)
      : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      var claims = new[]
      {
        new Claim(ClaimTypes.NameIdentifier, "integration-test"),
        new Claim(ClaimTypes.Name, "integration-test")
      };
      var identity = new ClaimsIdentity(claims, Scheme.Name);
      var principal = new ClaimsPrincipal(identity);
      var ticket = new AuthenticationTicket(principal, Scheme.Name);

      return Task.FromResult(AuthenticateResult.Success(ticket));
    }
  }
}

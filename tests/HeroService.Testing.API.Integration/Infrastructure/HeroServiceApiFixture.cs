using System.Security.Cryptography;
using HeroService.API.Authentication;
using HeroService.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace HeroService.Testing.API.Integration.Infrastructure;

public sealed class HeroServiceApiFixture : IAsyncLifetime
{
    private readonly string _databasePassword;
    private readonly PostgreSqlContainer _database;
    private readonly RedisContainer _cache;

    public HeroServiceApiFixture()
    {
        _databasePassword = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        _database = new PostgreSqlBuilder("postgres:16.10-alpine")
            .WithDatabase("hero_service_api_tests")
            .WithUsername("postgres")
            .WithPassword(_databasePassword)
            .Build();
        _cache = new RedisBuilder("redis:7.4.2-alpine").Build();
    }

    public WebApplicationFactory<Program> Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        await _cache.StartAsync();

        Factory = new HeroServiceApiFactory(_database, _cache, _databasePassword);

        using var startupClient = Factory.CreateClient();

        await using var scope = Factory.Services.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await database.Database.MigrateAsync();
    }

    public HttpClient CreateDevelopmentClient(string identity, params string[] roles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity);

        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Add(DevelopmentAuthenticationHandler.IdentityHeader, identity);

        if (roles.Length > 0)
        {
            client.DefaultRequestHeaders.Add(
                DevelopmentAuthenticationHandler.RolesHeader,
                string.Join(',', roles));
        }

        return client;
    }

    public async Task DisposeAsync()
    {
        if (Factory is not null)
            await Factory.DisposeAsync();

        await _cache.DisposeAsync();
        await _database.DisposeAsync();
    }

    private sealed class HeroServiceApiFactory : WebApplicationFactory<Program>
    {
        private readonly PostgreSqlContainer _database;
        private readonly RedisContainer _cache;
        private readonly string _databasePassword;

        public HeroServiceApiFactory(
            PostgreSqlContainer database,
            RedisContainer cache,
            string databasePassword)
        {
            _database = database;
            _cache = cache;
            _databasePassword = databasePassword;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, configuration) =>
            {
                var values = new Dictionary<string, string?>
                {
                    ["Database:Provider"] = "Postgres",
                    ["Database:IsolationMode"] = "SharedDatabase",
                    ["Database:ServerName"] = _database.Hostname,
                    ["Database:DatabaseName"] = "hero_service_api_tests",
                    ["Database:UserName"] = "postgres",
                    ["Database:Password"] = _databasePassword,
                    ["Database:Port"] = _database.GetMappedPublicPort(5432).ToString(),
                    ["Database:SslMode"] = "Disable",
                    ["Caching:ConnectionString"] = _cache.GetConnectionString(),
                    ["Caching:KeyPrefix"] = "HeroServiceTests"
                };

                configuration.AddInMemoryCollection(values);
            });
        }
    }
}

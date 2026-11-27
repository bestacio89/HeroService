using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using FluentAssertions;
using HeroService.API.Authentication;
using HeroService.API.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace HeroService.Testing.API.Integration;

public sealed class HeroServiceJwtAuthenticationTests : IAsyncLifetime
{
    private const string PlayerAuthority = "https://player.identity.test";
    private const string PlayerIssuer = "https://player.identity.test/";
    private const string PlayerAudience = "hero-service-player";
    private const string OperatorAuthority = "https://operator.identity.test";
    private const string OperatorIssuer = "https://operator.identity.test/";
    private const string OperatorAudience = "hero-service-operator";

    private readonly RSA _playerSigningKey = RSA.Create(2048);
    private readonly RSA _operatorSigningKey = RSA.Create(2048);
    private readonly RSA _untrustedSigningKey = RSA.Create(2048);
    private IHost _host = null!;
    private TestServer _server = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Player:Authority"] = PlayerAuthority,
                ["Authentication:Player:Issuer"] = PlayerIssuer,
                ["Authentication:Player:Audience"] = PlayerAudience,
                ["Authentication:Operator:Authority"] = OperatorAuthority,
                ["Authentication:Operator:Issuer"] = OperatorIssuer,
                ["Authentication:Operator:Audience"] = OperatorAudience
            })
            .Build();

        var environment = new TestHostEnvironment
        {
            EnvironmentName = Environments.Production
        };

        _host = new HostBuilder()
            .ConfigureWebHost(webHost => webHost
                .UseTestServer()
                .ConfigureServices(services =>
                {
                    services.AddLogging();
                    services.AddRouting();
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = HeroServiceAuthenticationSchemes.Player;
                        options.DefaultChallengeScheme = HeroServiceAuthenticationSchemes.Player;
                    });

                    services.AddHeroServiceAuthentication(environment, configuration);

                    services.Configure<JwtBearerOptions>(
                        HeroServiceAuthenticationSchemes.Player,
                        options => options.ConfigurationManager = CreateConfigurationManager(PlayerIssuer, _playerSigningKey));

                    services.Configure<JwtBearerOptions>(
                        HeroServiceAuthenticationSchemes.Operator,
                        options => options.ConfigurationManager = CreateConfigurationManager(OperatorIssuer, _operatorSigningKey));
                })
                .Configure(application =>
                {
                    application.UseRouting();
                    application.UseAuthentication();
                    application.UseAuthorization();
                    application.UseEndpoints(endpoints =>
                    {
                        endpoints.MapGet(
                                "/catalogue",
                                context => context.Response.WriteAsync("catalogue"))
                            .RequireAuthorization(HeroServiceAuthorizationPolicies.CatalogueRead);

                        endpoints.MapGet(
                                "/operator",
                                context => context.Response.WriteAsync("operator"))
                            .RequireAuthorization(HeroServiceAuthorizationPolicies.CatalogueWrite);
                    });
                }))
            .Build();

        await _host.StartAsync();

        _server = _host.GetTestServer();
        _client = _server.CreateClient();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ValidPlayerJwt_IsAcceptedForCatalogueReads()
    {
        using var request = CreateBearerRequest(
            CreateToken(PlayerIssuer, PlayerAudience, _playerSigningKey));

        using var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ValidOperatorJwt_IsAcceptedForOperatorWrites()
    {
        using var request = CreateBearerRequest(
            CreateToken(OperatorIssuer, OperatorAudience, _operatorSigningKey),
            "/operator");

        using var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("invalid-signature")]
    [InlineData("wrong-issuer")]
    [InlineData("wrong-audience")]
    [InlineData("expired")]
    [Trait("Category", "Integration")]
    public async Task InvalidPlayerJwt_IsRejected(string failure)
    {
        var issuer = failure == "wrong-issuer"
            ? "https://untrusted.identity.test/"
            : PlayerIssuer;
        var audience = failure == "wrong-audience"
            ? "another-api"
            : PlayerAudience;
        var signingKey = failure == "invalid-signature"
            ? _untrustedSigningKey
            : _playerSigningKey;
        var expires = failure == "expired"
            ? DateTime.UtcNow.AddMinutes(-5)
            : DateTime.UtcNow.AddMinutes(5);

        using var request = CreateBearerRequest(
            CreateToken(issuer, audience, signingKey, expires));

        using var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    private static HttpRequestMessage CreateBearerRequest(string token, string path = "/catalogue")
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static string CreateToken(
        string issuer,
        string audience,
        RSA signingKey,
        DateTime? expires = null)
    {
        var tokenExpires = expires ?? DateTime.UtcNow.AddMinutes(5);
        var notBefore = tokenExpires < DateTime.UtcNow
            ? tokenExpires.AddMinutes(-10)
            : DateTime.UtcNow.AddMinutes(-1);

        var securityKey = new RsaSecurityKey(signingKey)
        {
            KeyId = "hero-service-authentication-test-key"
        };
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
        var token = new JwtSecurityToken(
            issuer,
            audience,
            [new Claim("sub", "test-subject")],
            notBefore,
            tokenExpires,
            credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static IConfigurationManager<OpenIdConnectConfiguration> CreateConfigurationManager(
        string issuer,
        RSA signingKey)
    {
        var configuration = new OpenIdConnectConfiguration
        {
            Issuer = issuer
        };

        configuration.SigningKeys.Add(new RsaSecurityKey(signingKey)
        {
            KeyId = "hero-service-authentication-test-key"
        });

        return new StaticOpenIdConnectConfigurationManager(configuration);
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _host.StopAsync();
        _host.Dispose();
        _playerSigningKey.Dispose();
        _operatorSigningKey.Dispose();
        _untrustedSigningKey.Dispose();
    }

    private sealed class StaticOpenIdConnectConfigurationManager(
        OpenIdConnectConfiguration configuration) : IConfigurationManager<OpenIdConnectConfiguration>
    {
        public Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel)
        {
            cancel.ThrowIfCancellationRequested();
            return Task.FromResult(configuration);
        }

        public void RequestRefresh()
        {
        }
    }

    private sealed class TestHostEnvironment : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Production;

        public string ApplicationName { get; set; } = typeof(HeroServiceJwtAuthenticationTests).Assembly.GetName().Name!;

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}

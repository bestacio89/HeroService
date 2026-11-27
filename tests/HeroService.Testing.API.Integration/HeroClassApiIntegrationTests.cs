using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using HeroService.API.Authorization;
using HeroService.Testing.API.Integration.Infrastructure;
using Franz.Common.Http.MultiTenancy.Configuration;
using Franz.Common.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HeroService.Testing.API.Integration;

public sealed class HeroClassApiIntegrationTests : IClassFixture<HeroServiceApiFixture>
{
    private readonly HeroServiceApiFixture _fixture;

    public HeroClassApiIntegrationTests(HeroServiceApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task AnonymousRequests_AreRejected()
    {
        using var anonymousClient = _fixture.Factory.CreateClient();
        using var response = await anonymousClient.GetAsync("/api/v1/hero-classes");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PlayerCanReadHeroCatalogue()
    {
        using var playerClient = _fixture.CreateDevelopmentClient("player");
        using var response = await playerClient.GetAsync("/api/v1/hero-classes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        payload.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task OperatorCanReadHeroCatalogue()
    {
        using var operatorClient = _fixture.CreateDevelopmentClient("operator");
        using var response = await operatorClient.GetAsync("/api/v1/hero-classes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PlayerCannotCreateHero()
    {
        using var playerClient = _fixture.CreateDevelopmentClient("player");
        using var response = await playerClient.PostAsync(
            "/api/v1/heroes",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task OperatorCanReachCatalogueWriteEndpoint()
    {
        using var operatorClient = _fixture.CreateDevelopmentClient("operator");
        using var response = await operatorClient.PostAsync("/api/v1/heroes", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task PlayerCannotCreateOrActivateGameVersion()
    {
        using var playerClient = _fixture.CreateDevelopmentClient("player");
        using var response = await playerClient.PostAsync("/api/v1/game-versions", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task OperatorWithoutReleaseManagementRoleCannotCreateOrActivateGameVersion()
    {
        using var operatorClient = _fixture.CreateDevelopmentClient("operator");
        using var response = await operatorClient.PostAsync("/api/v1/game-versions", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task OperatorWithReleaseManagementRoleCanReachGameVersionEndpoint()
    {
        using var operatorClient = _fixture.CreateDevelopmentClient(
            "operator",
            HeroServiceAuthorizationPolicies.DefaultReleaseManagementRole);
        using var response = await operatorClient.PostAsync("/api/v1/game-versions", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetSkillLookups_ReturnsEmptyCollectionForUnknownIds()
    {
        using var playerClient = _fixture.CreateDevelopmentClient("player");
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();

        using var response = await playerClient.GetAsync(
            $"/api/v1/skills/lookup?ids={firstId}&ids={secondId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        payload.ValueKind.Should().Be(JsonValueKind.Array);
        payload.GetArrayLength().Should().Be(0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetHeroClassById_ReturnsNotFoundWhenMissing()
    {
        using var playerClient = _fixture.CreateDevelopmentClient("player");
        using var response = await playerClient.GetAsync($"/api/v1/hero-classes/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task BrowseSnapshots_ReturnsCompleteSkillKits_AndSingleSnapshotMatches()
    {
        using var playerClient = _fixture.CreateDevelopmentClient("player");
        using var browseResponse = await playerClient.GetAsync("/api/v1/snapshots/heroes/browse");

        browseResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var browsePayload = await browseResponse.Content.ReadFromJsonAsync<JsonElement>();
        browsePayload.ValueKind.Should().Be(JsonValueKind.Array);

        var snapshots = browsePayload.EnumerateArray().ToArray();
        snapshots.Should().NotBeEmpty();

        foreach (var snapshot in snapshots)
        {
            var skills = snapshot.GetProperty("skills");

            foreach (var slot in new[] { "passive", "primary", "secondary", "tertiary", "ultimate" })
            {
                var skill = skills.GetProperty(slot);

                skill.GetProperty("skillId").GetGuid().Should().NotBeEmpty();
                skill.TryGetProperty("effectExecutions", out var executions).Should().BeTrue();
                executions.ValueKind.Should().Be(JsonValueKind.Array);
            }
        }

        var firstSnapshot = snapshots[0];
        var heroId = firstSnapshot.GetProperty("heroId").GetGuid();
        var gameVersionId = firstSnapshot.GetProperty("gameVersionId").GetGuid();

        using var singleResponse = await playerClient.GetAsync(
            $"/api/v1/snapshots/heroes/{heroId}?gameVersionId={gameVersionId}");

        singleResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var singleSnapshot = await singleResponse.Content.ReadFromJsonAsync<JsonElement>();
        singleSnapshot.GetProperty("heroId").GetGuid().Should().Be(heroId);
        singleSnapshot.GetProperty("gameVersionId").GetGuid().Should().Be(gameVersionId);

        foreach (var slot in new[] { "passive", "primary", "secondary", "tertiary", "ultimate" })
        {
            var browseSkillId = firstSnapshot.GetProperty("skills").GetProperty(slot).GetProperty("skillId").GetGuid();
            var singleSkillId = singleSnapshot.GetProperty("skills").GetProperty(slot).GetProperty("skillId").GetGuid();

            singleSkillId.Should().Be(browseSkillId);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public void HttpArchitecture_DisablesTenantIsolationAndTenantEnforcement()
    {
        var multiTenancy = _fixture.Factory.Services
            .GetRequiredService<IOptions<MultiTenancyOptions>>().Value;
        var tenantResolution = _fixture.Factory.Services
            .GetRequiredService<IOptions<TenantResolutionOptions>>().Value;

        multiTenancy.Enabled.Should().BeFalse();
        tenantResolution.RequireTenant.Should().BeFalse();
        tenantResolution.RequireAuthenticatedAuthorization.Should().BeFalse();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task HealthLive_ReportsProcessLivenessWithoutDependencyChecks()
    {
        using var playerClient = _fixture.CreateDevelopmentClient("player");
        using var response = await playerClient.GetAsync("/health/live");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        payload.GetProperty("status").GetString().Should().Be("Healthy");
        payload.GetProperty("checks").GetArrayLength().Should().Be(0);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task HealthReady_ReportsTheMandatoryDatabaseDependency()
    {
        using var playerClient = _fixture.CreateDevelopmentClient("player");
        using var response = await playerClient.GetAsync("/health/ready");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<JsonElement>();
        payload.GetProperty("status").GetString().Should().Be("Healthy");

        var checks = payload.GetProperty("checks").EnumerateArray().ToArray();
        checks.Should().ContainSingle();
        checks[0].GetProperty("name").GetString().Should().Be("postgresql");
        checks[0].GetProperty("status").GetString().Should().Be("Healthy");
        payload.ToString().Should().NotContain("postgres:postgres");
    }
}

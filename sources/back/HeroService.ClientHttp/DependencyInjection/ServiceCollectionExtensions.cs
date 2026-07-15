using Microsoft.Extensions.DependencyInjection;
using HeroService.Client.Http.Abstractions;
using HeroService.Client.Http.Clients;

namespace HeroService.Client.Http.DependencyInjection;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddHeroServiceClients(
      this IServiceCollection services,
      string baseUrl,
      Action<HttpClient>? configureHttpClient = null)
  {
    if (string.IsNullOrWhiteSpace(baseUrl))
      throw new ArgumentException("Base URL must be provided.", nameof(baseUrl));

    // =========================================
    // CORE HERO CLIENTS
    // =========================================
    services.AddHttpClient<IHeroClient, HeroClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    // =========================================
    // CLASSIFICATIONS
    // =========================================
    services.AddHttpClient<IHeroClassClient, HeroClassClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    services.AddHttpClient<IMythologyTypeClient, MythologyTypeClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    services.AddHttpClient<IOriginCultureClient, OriginCultureClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    services.AddHttpClient<IOriginArchetypeClient, OriginArchetypeClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    // =========================================
    // PROGRESSION SYSTEM
    // =========================================
    services.AddHttpClient<IHeroProgressionClient, HeroProgressionClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    // =========================================
    // SKILLS
    // =========================================
    services.AddHttpClient<ISkillClient, SkillClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    services.AddHttpClient<ISkillScalingClient, SkillScalingClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    // =========================================
    // SNAPSHOTS (CRITICAL SYSTEM)
    // =========================================
    services.AddHttpClient<IHeroSnapshotClient, HeroSnapshotClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    // =========================================
    // VERSIONING
    // =========================================
    services.AddHttpClient<IGameVersionClient, GameVersionClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });

    ///=======================================
    /// MODIFIERS
    ///=======================================
    services.AddHttpClient<IHeroModifierClient, HeroModifierClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });
    services.AddHttpClient<ISkillModifierClient, SkillModifierClient>(client =>
    {
      Configure(client, baseUrl, configureHttpClient);
    });


    return services;

    
  }

  private static void Configure(
      HttpClient client,
      string baseUrl,
      Action<HttpClient>? configureHttpClient)
  {
    client.BaseAddress = new Uri(baseUrl);

    client.DefaultRequestHeaders.Add("X-Client", "HeroService.Admin");

    configureHttpClient?.Invoke(client);
  }
}
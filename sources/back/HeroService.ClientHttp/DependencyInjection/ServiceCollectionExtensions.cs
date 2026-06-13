using Microsoft.Extensions.DependencyInjection;
using HeroService.Client.Http.Abstractions;
using HeroService.Client.Http.Clients;

namespace HeroService.Client.Http.DependencyInjection;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddHeroServiceClients(
      this IServiceCollection services,
      Action<HttpClient>? configureHttpClient = null)
  {
    // =========================================
    // CORE CONFIGURATION (optional shared config)
    // =========================================
    services.AddHttpClient("HeroService", client =>
    {
      configureHttpClient?.Invoke(client);
    });

    // =========================================
    // HERO CORE
    // =========================================
    services.AddHttpClient<IHeroClient, HeroClient>("HeroService");

    // =========================================
    // CLASSIFICATIONS
    // =========================================
    services.AddHttpClient<IHeroClassClient, HeroClassClient>("HeroService");
    services.AddHttpClient<IMythologyTypeClient, MythologyTypeClient>("HeroService");
    services.AddHttpClient<IOriginCultureClient, OriginCultureClient>("HeroService");
    services.AddHttpClient<IOriginArchetypeClient, OriginArchetypeClient>("HeroService");

    // =========================================
    // HERO SYSTEMS
    // =========================================
    services.AddHttpClient<IHeroProgressionClient, HeroProgressionClient>("HeroService");

    // =========================================
    // SKILLS
    // =========================================
    services.AddHttpClient<ISkillClient, SkillClient>("HeroService");
    services.AddHttpClient<ISkillScalingClient, SkillScalingClient>("HeroService");

    // =========================================
    // SNAPSHOTS (UNITY CRITICAL)
    // =========================================
    services.AddHttpClient<IHeroSnapshotClient, HeroSnapshotClient>("HeroService");

    // =========================================
    // GAME VERSIONING
    // =========================================
    services.AddHttpClient<IGameVersionClient, GameVersionClient>("HeroService");

    return services;
  }
}
using Franz.Common.Caching.Abstractions;
using Franz.Common.Caching.Estrategies;
using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.MultiTenancy.Configuration;
using Franz.Common.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Microsoft.Extensions.Hosting;

namespace HeroService.API.Extensions;

public static class HeroServiceHttpArchitectureExtensions
{
  public static IServiceCollection AddHeroServiceHttpArchitecture(
      this IServiceCollection services,
      IHostEnvironment hostEnvironment,
      IConfiguration configuration)
  {
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(hostEnvironment);
    ArgumentNullException.ThrowIfNull(configuration);

    services.AddHttpArchitecture(hostEnvironment, configuration);

    // HeroService is monotenant, so cache keys must not be tenant-prefixed.
    // Resolve configured options explicitly because Franz.Common 3.0 exposes two DI-resolvable constructors.
    services.Replace(ServiceDescriptor.Singleton<ICacheKeyStrategy>(serviceProvider =>
        new DefaultCacheKeyStrategy(
            serviceProvider.GetRequiredService<IOptions<JsonSerializerOptions>>())));

    // The shared HTTP bootstrap includes tenant infrastructure. HeroService is
    // a single-tenant service, so retain the standard HTTP pipeline while
    // explicitly disabling tenant isolation and tenant authorization.
    services.Configure<MultiTenancyOptions>(options => options.Enabled = false);
    services.Configure<TenantResolutionOptions>(options =>
    {
      options.RequireTenant = false;
      options.RequireAuthenticatedAuthorization = false;
    });

    return services;
  }
}

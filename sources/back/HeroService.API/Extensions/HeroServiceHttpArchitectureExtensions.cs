using Franz.Common.Http.Bootstrap.Extensions;
using Franz.Common.Http.MultiTenancy.Configuration;
using Franz.Common.MultiTenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

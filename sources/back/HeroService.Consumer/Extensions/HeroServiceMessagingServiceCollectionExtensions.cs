using Franz.Common.Bootstrap.Extensions;
using Franz.Common.Messaging.Extensions;
using Franz.Common.Messaging.Identity.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeroService.Consumer.Extensions;

public static class HeroServiceMessagingServiceCollectionExtensions
{
  public static IServiceCollection AddHeroServiceMessagingArchitecture(
      this IServiceCollection services,
      IConfiguration configuration)
  {
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(configuration);

    return services
        .AddCommonArchitecture(configuration, typeof(HeroService.Consumer.Program).Assembly)
        .AddMessagingIdentityContext()
        .AddMessagingHeaderContext();
  }
}

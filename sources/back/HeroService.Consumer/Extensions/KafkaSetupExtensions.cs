using Franz.Common.Messaging.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeroService.Consumer.Extensions;

public static class KafkaSetupExtensions
{
  public static IServiceCollection AddKafkaMessagingServices(
      this IServiceCollection services,
      IConfiguration configuration,
      string environmentName)
  {
    KafkaDeploymentConfigurationValidator.Validate(configuration, environmentName);
    services.AddKafkaMessaging(configuration);
    return services;
  }
}

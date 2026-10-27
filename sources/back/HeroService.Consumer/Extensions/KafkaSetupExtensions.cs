using Franz.Common.Messaging.Hosting.Kafka;
using Franz.Common.Messaging.Kafka.Configuration;
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

    var inputTopics = configuration
        .GetSection("Messaging:Kafka:Consumer:Topics")
        .Get<string[]>() ?? [];

    services.AddKafkaMessaging(configuration);
    services.AddSingleton<IEnumerable<string>>(_ => inputTopics);

    services.AddKafkaHostedListener(options =>
        configuration.GetSection(KafkaMessagingOptions.SectionName).Bind(options));

    return services;
  }
}

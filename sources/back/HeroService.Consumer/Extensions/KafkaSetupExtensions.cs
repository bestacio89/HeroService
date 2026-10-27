using Franz.Common.Messaging.Hosting.Kafka;
using Franz.Common.Messaging.Kafka.Configuration;
using Franz.Common.Messaging.Kafka.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

    services.AddSingleton<IEnumerable<string>>(serviceProvider =>
    {
      var options = serviceProvider.GetRequiredService<IOptions<KafkaMessagingOptions>>().Value;
      if (string.IsNullOrWhiteSpace(options.TopicName))
        throw new InvalidOperationException("Messaging:Kafka:TopicName must be configured before the Kafka listener starts.");

      return new[] { options.TopicName };
    });

    services.AddKafkaHostedListener(options =>
        configuration.GetSection(KafkaMessagingOptions.SectionName).Bind(options));

    return services;
  }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Franz.Common.Messaging.Kafka;
using Franz.Common.Messaging.Kafka.Extensions;

namespace HeroService.Consumer.Extensions
{
  public static class KafkaSetupExtensions
  {
    public static IServiceCollection AddKafkaMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
      // Register Kafka-related services
      services.AddKafkaMessaging(configuration);
      return services;
    }
  }
}

using Franz.Common.Messaging.Hosting.Kafka.HostedServices;
using HeroService.Consumer.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HeroService.Testing.Consumer;

public sealed class KafkaHostRegistrationTests
{
  [Fact]
  public void Registration_UsesConfiguredInputTopicsAndRegistersHostedListener()
  {
    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["Messaging:Kafka:BootstrapServers"] = "localhost:9092",
        ["Messaging:Kafka:GroupId"] = "hero-service-consumer-test",
        ["Messaging:Kafka:TopicName"] = "hero-service-out-test",
        ["Messaging:Kafka:Consumer:Topics:0"] = "hero-service-in-test",
        ["Messaging:Kafka:Security:SecurityProtocol"] = "Plaintext"
      })
      .Build();

    var services = new ServiceCollection();
    services.AddLogging();
    services.AddSingleton<IConfiguration>(configuration);
    services.AddKafkaMessagingServices(configuration, "Development");

    using var provider = services.BuildServiceProvider();

    var topics = provider.GetRequiredService<IEnumerable<string>>();
    Assert.Equal(new[] { "hero-service-in-test" }, topics);

    var hostedServices = provider.GetServices<IHostedService>();
    Assert.Contains(hostedServices, service => service is KafkaMessagingHostedService);

    var listener = provider.GetRequiredService<KafkaMessageListener>();
    Assert.NotNull(listener);
  }
}

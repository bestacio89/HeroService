using Confluent.Kafka;
using Confluent.Kafka.Admin;
using HeroService.Consumer.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Testcontainers.Kafka;

namespace HeroService.Testing.Consumer;

public sealed class KafkaBrokerHealthCheckTests : IAsyncLifetime
{
  private readonly KafkaContainer _kafka =
      new KafkaBuilder("confluentinc/cp-kafka:7.4.0").WithCleanUp(true).Build();

  public Task InitializeAsync() => _kafka.StartAsync();

  public Task DisposeAsync() => _kafka.DisposeAsync().AsTask();

  [Fact]
  public async Task Readiness_RequiresBrokerAndAllConfiguredTopics()
  {
    using var admin = new AdminClientBuilder(new AdminClientConfig
    {
      BootstrapServers = _kafka.GetBootstrapAddress()
    }).Build();

    await admin.CreateTopicsAsync(new[]
    {
      new TopicSpecification { Name = "hero-health-in", NumPartitions = 1, ReplicationFactor = 1 },
      new TopicSpecification { Name = "hero-health-out", NumPartitions = 1, ReplicationFactor = 1 }
    });

    var configuration = new ConfigurationBuilder()
      .AddInMemoryCollection(new Dictionary<string, string?>
      {
        ["Messaging:Kafka:TopicName"] = "hero-health-out",
        ["Messaging:Kafka:Consumer:Topics:0"] = "hero-health-in",
        ["Messaging:Kafka:Failure:DeadLetterTopic"] = "hero-health-dlt"
      })
      .Build();

    var check = new KafkaBrokerHealthCheck(admin, configuration);

    var notReady = await check.CheckHealthAsync(new HealthCheckContext());
    Assert.Equal(HealthStatus.Unhealthy, notReady.Status);

    await admin.CreateTopicsAsync(new[]
    {
      new TopicSpecification { Name = "hero-health-dlt", NumPartitions = 1, ReplicationFactor = 1 }
    });

    var ready = await check.CheckHealthAsync(new HealthCheckContext());
    Assert.Equal(HealthStatus.Healthy, ready.Status);
  }
}

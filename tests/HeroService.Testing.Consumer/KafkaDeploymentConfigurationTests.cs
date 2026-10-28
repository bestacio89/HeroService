using HeroService.Consumer.Extensions;
using Microsoft.Extensions.Configuration;

namespace HeroService.Testing.Consumer;

public sealed class KafkaDeploymentConfigurationTests
{
  [Fact]
  public void Development_AllowsLocalPlaintextBroker()
  {
    var configuration = BuildConfiguration(new Dictionary<string, string?>
    {
      ["Messaging:Kafka:BootstrapServers"] = "localhost:9092",
      ["Messaging:Kafka:GroupId"] = "hero-service-consumer-development",
      ["Messaging:Kafka:TopicName"] = "hero-service-out-development",
      ["Messaging:Kafka:Consumer:Topics:0"] = "hero-service-in-development",
      ["Messaging:Kafka:Security:SecurityProtocol"] = "Plaintext"
    });

    KafkaDeploymentConfigurationValidator.Validate(configuration, "Development");
  }

  [Fact]
  public void Production_RejectsPlaintextSecurity()
  {
    var values = BuildProductionConfiguration();
    values["Messaging:Kafka:Security:SecurityProtocol"] = "Plaintext";

    Assert.Throws<InvalidOperationException>(
        () => KafkaDeploymentConfigurationValidator.Validate(BuildConfiguration(values), "Production"));
  }

  [Fact]
  public void Production_RejectsLocalBootstrapAddress()
  {
    var values = BuildProductionConfiguration();
    values["Messaging:Kafka:BootstrapServers"] = "localhost:9092";

    Assert.Throws<InvalidOperationException>(
        () => KafkaDeploymentConfigurationValidator.Validate(BuildConfiguration(values), "Production"));
  }

  [Fact]
  public void Production_RejectsLocalDatabase()
  {
    var values = BuildProductionConfiguration();
    values["Database:ServerName"] = "localhost";

    Assert.Throws<InvalidOperationException>(
        () => KafkaDeploymentConfigurationValidator.Validate(BuildConfiguration(values), "Production"));
  }

  [Fact]
  public void Production_RejectsLocalRedis()
  {
    var values = BuildProductionConfiguration();
    values["Caching:ConnectionString"] = "localhost:6379";

    Assert.Throws<InvalidOperationException>(
        () => KafkaDeploymentConfigurationValidator.Validate(BuildConfiguration(values), "Production"));
  }

  [Fact]
  public void Production_RejectsAutomaticOffsetCommit()
  {
    var values = BuildProductionConfiguration();
    values["Messaging:Kafka:Consumer:EnableAutoCommit"] = "true";

    Assert.Throws<InvalidOperationException>(
        () => KafkaDeploymentConfigurationValidator.Validate(BuildConfiguration(values), "Production"));
  }

  [Fact]
  public void Production_RejectsInputOutputTopicCollision()
  {
    var values = BuildProductionConfiguration();
    values["Messaging:Kafka:Consumer:Topics:0"] = values["Messaging:Kafka:TopicName"];

    Assert.Throws<InvalidOperationException>(
        () => KafkaDeploymentConfigurationValidator.Validate(BuildConfiguration(values), "Production"));
  }

  [Fact]
  public void Production_AcceptsTlsSaslAndExplicitDeliveryGuarantees()
  {
    KafkaDeploymentConfigurationValidator.Validate(
        BuildConfiguration(BuildProductionConfiguration()), "Production");
  }

  private static Dictionary<string, string?> BuildProductionConfiguration() => new()
  {
    ["Database:ServerName"] = "postgres.example.net",
    ["Database:DatabaseName"] = "GameProject",
    ["Database:UserName"] = "hero-service",
    ["Database:Password"] = "injected-db-secret",
    ["Database:SslMode"] = "Require",
    ["Caching:ConnectionString"] = "redis.example.net:6380,ssl=true",
    ["Messaging:Kafka:BootstrapServers"] = "broker.example.net:9093",
    ["Messaging:Kafka:GroupId"] = "hero-service-consumer-production",
    ["Messaging:Kafka:TopicName"] = "hero-service-out",
    ["Messaging:Kafka:Consumer:Topics:0"] = "hero-service-in",
    ["Messaging:Kafka:Security:SecurityProtocol"] = "SaslSsl",
    ["Messaging:Kafka:Security:SaslMechanism"] = "ScramSha512",
    ["Messaging:Kafka:Security:SaslUsername"] = "injected-user",
    ["Messaging:Kafka:Security:SaslPassword"] = "injected-secret",
    ["Messaging:Kafka:Consumer:EnableAutoCommit"] = "false",
    ["Messaging:Kafka:Consumer:EnableAutoOffsetStore"] = "false",
    ["Messaging:Kafka:Failure:EnableDeadLetter"] = "true",
    ["Messaging:Kafka:Producer:EnableIdempotence"] = "true",
    ["Messaging:Kafka:Producer:Acks"] = "All"
  };

  private static IConfiguration BuildConfiguration(IDictionary<string, string?> values)
    => new ConfigurationBuilder().AddInMemoryCollection(values).Build();
}

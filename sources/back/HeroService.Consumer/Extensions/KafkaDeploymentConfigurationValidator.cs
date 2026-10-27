using Microsoft.Extensions.Configuration;

namespace HeroService.Consumer.Extensions;

public static class KafkaDeploymentConfigurationValidator
{
  public static void Validate(IConfiguration configuration, string environmentName)
  {
    ArgumentNullException.ThrowIfNull(configuration);

    var bootstrapServers = configuration["Messaging:Kafka:BootstrapServers"];
    if (string.IsNullOrWhiteSpace(bootstrapServers))
      throw new InvalidOperationException(
          "Messaging:Kafka:BootstrapServers must be supplied by environment-specific configuration or deployment secrets.");

    var groupId = configuration["Messaging:Kafka:GroupId"];
    if (string.IsNullOrWhiteSpace(groupId))
      throw new InvalidOperationException(
          "Messaging:Kafka:GroupId must be configured at the Kafka options root.");

    var topicName = configuration["Messaging:Kafka:TopicName"];
    if (string.IsNullOrWhiteSpace(topicName))
      throw new InvalidOperationException(
          "Messaging:Kafka:TopicName must identify the explicitly configured HeroService topic.");

    var isDevelopment = string.Equals(
        environmentName, "Development", StringComparison.OrdinalIgnoreCase);

    if (isDevelopment)
      return;

    if (bootstrapServers.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Any(server =>
            server.Contains("localhost", StringComparison.OrdinalIgnoreCase) ||
            server.Contains("127.0.0.1", StringComparison.OrdinalIgnoreCase) ||
            server.StartsWith("0.0.0.0", StringComparison.OrdinalIgnoreCase)))
    {
      throw new InvalidOperationException(
          "Production Kafka bootstrap servers must not point to a loopback or wildcard address.");
    }

    var securityProtocol = configuration["Messaging:Kafka:Security:SecurityProtocol"];
    if (!string.Equals(securityProtocol, "Ssl", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(securityProtocol, "SaslSsl", StringComparison.OrdinalIgnoreCase))
    {
      throw new InvalidOperationException(
          "Kafka must use Ssl or SaslSsl outside the Development environment.");
    }

    if (string.Equals(securityProtocol, "SaslSsl", StringComparison.OrdinalIgnoreCase) &&
        (string.IsNullOrWhiteSpace(configuration["Messaging:Kafka:Security:SaslMechanism"]) ||
         string.IsNullOrWhiteSpace(configuration["Messaging:Kafka:Security:SaslUsername"]) ||
         string.IsNullOrWhiteSpace(configuration["Messaging:Kafka:Security:SaslPassword"])))
    {
      throw new InvalidOperationException(
          "SaslSsl requires a SASL mechanism, username and password supplied through secure configuration.");
    }

    if (bool.TryParse(configuration["Messaging:Kafka:Consumer:EnableAutoCommit"], out var autoCommit) && autoCommit)
      throw new InvalidOperationException("Kafka auto-commit must remain disabled so processing controls acknowledgements.");

    if (bool.TryParse(configuration["Messaging:Kafka:Consumer:EnableAutoOffsetStore"], out var autoOffsetStore) && autoOffsetStore)
      throw new InvalidOperationException("Kafka auto-offset-store must remain disabled so only completed processing advances offsets.");

    if (!bool.TryParse(configuration["Messaging:Kafka:Failure:EnableDeadLetter"], out var deadLetterEnabled) || !deadLetterEnabled)
      throw new InvalidOperationException("Kafka dead-letter publishing must be enabled outside Development.");

    if (!bool.TryParse(configuration["Messaging:Kafka:Producer:EnableIdempotence"], out var idempotence) || !idempotence)
      throw new InvalidOperationException("Kafka producer idempotence must be enabled outside Development.");

    if (!string.Equals(configuration["Messaging:Kafka:Producer:Acks"], "All", StringComparison.OrdinalIgnoreCase))
      throw new InvalidOperationException("Kafka producer acknowledgements must be set to All outside Development.");
  }
}

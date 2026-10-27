using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HeroService.Consumer.HealthChecks;

public sealed class KafkaBrokerHealthCheck : IHealthCheck
{
  private readonly IAdminClient _adminClient;
  private readonly IConfiguration _configuration;

  public KafkaBrokerHealthCheck(IAdminClient adminClient, IConfiguration configuration)
  {
    _adminClient = adminClient ?? throw new ArgumentNullException(nameof(adminClient));
    _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
  }

  public Task<HealthCheckResult> CheckHealthAsync(
      HealthCheckContext context,
      CancellationToken cancellationToken = default)
  {
    try
    {
      cancellationToken.ThrowIfCancellationRequested();

      var metadata = _adminClient.GetMetadata(TimeSpan.FromSeconds(3));
      if (metadata.Brokers.Count == 0)
        return Task.FromResult(HealthCheckResult.Unhealthy("Kafka returned no broker metadata."));

      var inputTopics = _configuration
          .GetSection("Messaging:Kafka:Consumer:Topics")
          .Get<string[]>() ?? [];

      var outputTopic = _configuration["Messaging:Kafka:TopicName"];
      var configuredDeadLetterTopic = _configuration["Messaging:Kafka:Failure:DeadLetterTopic"];

      var requiredTopics = inputTopics
          .Append(outputTopic)
          .Where(topic => !string.IsNullOrWhiteSpace(topic))
          .Select(topic => topic!)
          .Concat(string.IsNullOrWhiteSpace(configuredDeadLetterTopic)
              ? inputTopics.Select(topic => $"{topic}-dlt")
              : [configuredDeadLetterTopic])
          .Distinct(StringComparer.Ordinal)
          .ToArray();

      var topicMetadata = metadata.Topics.ToDictionary(topic => topic.Topic, StringComparer.Ordinal);
      var missingTopics = requiredTopics
          .Where(topic =>
              !topicMetadata.TryGetValue(topic, out var topicInfo) ||
              topicInfo.Error.IsError ||
              topicInfo.Partitions.Count == 0)
          .ToArray();

      if (missingTopics.Length > 0)
      {
        return Task.FromResult(HealthCheckResult.Unhealthy(
            $"Kafka is reachable, but required topics are missing or unavailable: {string.Join(", ", missingTopics)}."));
      }

      return Task.FromResult(HealthCheckResult.Healthy("Kafka brokers and required topics are available."));
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
      throw;
    }
    catch (Exception exception)
    {
      return Task.FromResult(HealthCheckResult.Unhealthy(
          "Kafka broker metadata could not be retrieved.", exception));
    }
  }
}

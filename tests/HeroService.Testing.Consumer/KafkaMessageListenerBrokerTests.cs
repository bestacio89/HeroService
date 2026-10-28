using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Franz.Common.Messaging.Hosting;
using Franz.Common.Messaging.Hosting.Kafka.HostedServices;
using Franz.Common.Messaging.Kafka.Configuration;
using Franz.Common.Messaging.Messages;
using Franz.Common.Messaging.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Testcontainers.Kafka;

namespace HeroService.Testing.Consumer;

/// <summary>
/// Exercises the released Franz.Common listener against a real Kafka broker.
/// These tests intentionally use unique topics and consumer groups so broker state
/// cannot leak between test cases.
/// </summary>
public sealed class KafkaMessageListenerBrokerTests
{
  [Fact]
  public async Task SuccessfulHandler_CommitsRecord_AndRestartDoesNotRedeliver()
  {
    await using var kafka = await StartKafkaAsync();
    var topic = NewTopic("hero-success");
    await CreateTopicsAsync(kafka, topic);

    var groupId = $"hero-success-{Guid.NewGuid():N}";
    await ProduceMessageAsync(kafka, topic, new Message("success"));

    using var cancellation = new CancellationTokenSource();
    var handled = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    var listener = CreateListener(kafka, topic, groupId, new KafkaFailureOptions
    {
      EnableDeadLetter = false,
      MaxRetryAttempts = 0
    });

    listener.OnMessageReceivedAsync = args =>
    {
      Assert.Equal("success", args.Message.Body);
      handled.TrySetResult();
      return Task.CompletedTask;
    };

    var listening = listener.Listen(cancellation.Token);
    await handled.Task.WaitAsync(TimeSpan.FromSeconds(20));
    cancellation.Cancel();
    await listening.WaitAsync(TimeSpan.FromSeconds(20));

    using var restartedConsumer = BuildConsumer(kafka, groupId);
    restartedConsumer.Subscribe(topic);
    var redelivery = restartedConsumer.Consume(TimeSpan.FromSeconds(3));

    Assert.Null(redelivery);
  }

  [Fact]
  public async Task HandlerFailureWithoutDeadLetter_DoesNotCommit_AndRecordIsRedelivered()
  {
    await using var kafka = await StartKafkaAsync();
    var topic = NewTopic("hero-failure");
    await CreateTopicsAsync(kafka, topic);

    var groupId = $"hero-failure-{Guid.NewGuid():N}";
    var payload = new Message("must-be-redelivered");
    await ProduceMessageAsync(kafka, topic, payload);

    using var deadLetterProducer = BuildDeadLetterProducer(kafka);
    var listener = CreateListener(kafka, topic, groupId, new KafkaFailureOptions
    {
      EnableDeadLetter = false,
      MaxRetryAttempts = 0,
      RetryBackoffMs = 0
    }, deadLetterProducer);
    listener.OnMessageReceivedAsync = _ => Task.FromException(new InvalidOperationException("Expected test failure."));

    var failure = await Record.ExceptionAsync(() => listener.Listen().WaitAsync(TimeSpan.FromSeconds(20)));
    Assert.IsType<InvalidOperationException>(failure);
    Assert.Contains("offset was not committed", failure!.Message, StringComparison.OrdinalIgnoreCase);

    using var restartedConsumer = BuildConsumer(kafka, groupId);
    restartedConsumer.Subscribe(topic);
    var redelivery = restartedConsumer.Consume(TimeSpan.FromSeconds(15));

    Assert.NotNull(redelivery);
    Assert.Equal(topic, redelivery.Topic);
    Assert.Equal("must-be-redelivered", new JsonMessageSerializer().Deserialize<Message>(redelivery.Message.Value)!.Body);
  }

  [Fact]
  public async Task ExhaustedRetries_PublishesDeadLetterWithSourceMetadata_ThenCommits()
  {
    await using var kafka = await StartKafkaAsync();
    var topic = NewTopic("hero-dlt-source");
    var deadLetterTopic = $"{topic}-dlt";
    await CreateTopicsAsync(kafka, topic, deadLetterTopic);

    var groupId = $"hero-dlt-{Guid.NewGuid():N}";
    await ProduceMessageAsync(kafka, topic, new Message("poison"));

    using var producer = BuildDeadLetterProducer(kafka);
    var listener = CreateListener(
        kafka,
        topic,
        groupId,
        new KafkaFailureOptions
        {
          EnableDeadLetter = true,
          DeadLetterTopic = deadLetterTopic,
          MaxRetryAttempts = 1,
          RetryBackoffMs = 0,
          UseOriginalMessageKey = true
        },
        producer);

    var attempts = 0;
    listener.OnMessageReceivedAsync = _ =>
    {
      Interlocked.Increment(ref attempts);
      return Task.FromException(new InvalidOperationException("Poison message."));
    };

    using var cancellation = new CancellationTokenSource();
    var listening = listener.Listen(cancellation.Token);

    using var deadLetterConsumer = BuildConsumer(kafka, $"dlt-reader-{Guid.NewGuid():N}");
    deadLetterConsumer.Subscribe(deadLetterTopic);
    var deadLetter = deadLetterConsumer.Consume(TimeSpan.FromSeconds(20));

    Assert.NotNull(deadLetter);
    Assert.Equal("poison", new JsonMessageSerializer().Deserialize<Message>(deadLetter.Message.Value)!.Body);
    Assert.Equal(2, attempts);
    Assert.Equal(topic, HeaderValue(deadLetter.Message.Headers, "x-franz-original-topic"));
    Assert.Equal("0", HeaderValue(deadLetter.Message.Headers, "x-franz-original-offset"));
    Assert.Equal(typeof(InvalidOperationException).FullName, HeaderValue(deadLetter.Message.Headers, "x-franz-failure-type"));

    cancellation.Cancel();
    await listening.WaitAsync(TimeSpan.FromSeconds(20));

    using var restartedConsumer = BuildConsumer(kafka, groupId);
    restartedConsumer.Subscribe(topic);
    var redelivery = restartedConsumer.Consume(TimeSpan.FromSeconds(3));
    Assert.Null(redelivery);
  }

  private static async Task<KafkaContainer> StartKafkaAsync()
  {
    var kafka = new KafkaBuilder("confluentinc/cp-kafka:7.4.0")
      .WithCleanUp(true)
      .Build();

    await kafka.StartAsync();
    return kafka;
  }

  private static async Task CreateTopicsAsync(KafkaContainer kafka, params string[] topics)
  {
    using var admin = new AdminClientBuilder(new AdminClientConfig
    {
      BootstrapServers = kafka.GetBootstrapAddress()
    }).Build();

    await admin.CreateTopicsAsync(topics.Select(topic => new TopicSpecification
    {
      Name = topic,
      NumPartitions = 1,
      ReplicationFactor = 1
    }));
  }

  private static async Task ProduceMessageAsync(KafkaContainer kafka, string topic, Message message)
  {
    using var producer = new ProducerBuilder<string, string>(new ProducerConfig
    {
      BootstrapServers = kafka.GetBootstrapAddress(),
      Acks = Acks.All,
      EnableIdempotence = true
    }).Build();

    await producer.ProduceAsync(topic, new Confluent.Kafka.Message<string, string>
    {
      Key = message.Id.ToString(),
      Value = new JsonMessageSerializer().Serialize(message)
    });
  }

  private static IProducer<string, byte[]> BuildDeadLetterProducer(KafkaContainer kafka)
    => new ProducerBuilder<string, byte[]>(new ProducerConfig
    {
      BootstrapServers = kafka.GetBootstrapAddress(),
      Acks = Acks.All,
      EnableIdempotence = true
    }).Build();

  private static KafkaMessageListener CreateListener(
      KafkaContainer kafka,
      string topic,
      string groupId,
      KafkaFailureOptions failureOptions,
      IProducer<string, byte[]>? deadLetterProducer = null)
  {
    var consumer = BuildConsumer(kafka, groupId);
    var serializer = new JsonMessageSerializer();
    var logger = NullLogger<KafkaMessageListener>.Instance;

    if (deadLetterProducer is null)
      return new KafkaMessageListener(consumer, new[] { topic }, serializer, logger);

    var options = Options.Create(new KafkaMessagingOptions
    {
      BootstrapServers = kafka.GetBootstrapAddress(),
      GroupId = groupId,
      Consumer = new KafkaConsumerOptions(),
      Producer = new KafkaProducerOptions(),
      Failure = failureOptions
    });

    return new KafkaMessageListener(consumer, new[] { topic }, serializer, logger, deadLetterProducer, options);
  }

  private static IConsumer<string, string> BuildConsumer(KafkaContainer kafka, string groupId)
    => new ConsumerBuilder<string, string>(new ConsumerConfig
    {
      BootstrapServers = kafka.GetBootstrapAddress(),
      GroupId = groupId,
      AutoOffsetReset = AutoOffsetReset.Earliest,
      EnableAutoCommit = false,
      EnableAutoOffsetStore = false,
      AllowAutoCreateTopics = false
    }).Build();

  private static string NewTopic(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

  private static string? HeaderValue(Headers? headers, string key)
  {
    var header = headers?.FirstOrDefault(item => item.Key == key);
    return header is null ? null : System.Text.Encoding.UTF8.GetString(header.GetValueBytes());
  }
}

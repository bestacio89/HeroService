using Franz.Common.Messaging.Messages;
using HeroService.Consumer.Processing;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HeroService.Testing.Consumer;

public sealed class GenericMessageProcessorTests
{
  [Fact]
  public void Process_WhenBodyIsMalformed_PropagatesDeserializationFailure()
  {
    var logger = new CapturingLogger<GenericMessageProcessor<TestPayload>>();
    var processor = new GenericMessageProcessor<TestPayload>(logger);
    var message = new Message("{");

    Assert.Throws<JsonReaderException>(() => processor.Process(message));
    Assert.Contains(logger.Messages, entry => entry.StartsWith("Error:", StringComparison.Ordinal));
  }

  [Fact]
  public void Process_WhenBodyIsJsonNull_ReportsPayloadWasNotApplied()
  {
    var logger = new CapturingLogger<GenericMessageProcessor<TestPayload>>();
    var processor = new GenericMessageProcessor<TestPayload>(logger);

    processor.Process(new Message("null"));

    Assert.Contains(logger.Messages, entry =>
      entry.StartsWith("Warning:", StringComparison.Ordinal) &&
      entry.Contains("could not be deserialized", StringComparison.Ordinal));
    Assert.DoesNotContain(logger.Messages, entry =>
      entry.Contains("Successfully processed", StringComparison.Ordinal));
  }

  [Fact]
  public void Process_WhenPayloadDeserializes_ExplicitlyReportsNoBusinessAction()
  {
    var logger = new CapturingLogger<GenericMessageProcessor<TestPayload>>();
    var processor = new GenericMessageProcessor<TestPayload>(logger);

    processor.Process(new Message("{\"name\":\"test\"}"));

    Assert.Contains(logger.Messages, entry =>
      entry.StartsWith("Warning:", StringComparison.Ordinal) &&
      entry.Contains("payload was not applied", StringComparison.Ordinal));
    Assert.DoesNotContain(logger.Messages, entry =>
      entry.Contains("Successfully processed", StringComparison.Ordinal));
  }

  private sealed class TestPayload
  {
    public string? Name { get; set; }
  }

  private sealed class CapturingLogger<T> : ILogger<T>
  {
    public List<string> Messages { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
      LogLevel logLevel,
      EventId eventId,
      TState state,
      Exception? exception,
      Func<TState, Exception?, string> formatter)
    {
      Messages.Add($"{logLevel}:{formatter(state, exception)}");
    }
  }
}

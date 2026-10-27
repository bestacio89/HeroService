using Franz.Common.Messaging.Messages;
using HeroService.Consumer.Processing;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HeroService.Testing.Consumer;

public sealed class GenericMessageProcessorTests
{
  [Fact]
  public void Process_WhenBodyIsMalformed_PropagatesDeserializationFailure()
  {
    var logger = new CapturingLogger<GenericMessageProcessor<GameVersion>>();
    var processor = new GenericMessageProcessor<GameVersion>(logger);
    var message = new Message("{");

    Assert.Throws<JsonReaderException>(() => processor.Process(message));
    Assert.Contains(logger.Messages, entry => entry.StartsWith("Error:", StringComparison.Ordinal));
  }

  [Fact]
  public void Process_WhenBodyIsJsonNull_ReportsPayloadWasNotApplied()
  {
    var logger = new CapturingLogger<GenericMessageProcessor<GameVersion>>();
    var processor = new GenericMessageProcessor<GameVersion>(logger);
    var message = new Message("null");

    processor.Process(message);

    Assert.Contains(logger.Messages, entry =>
      entry.StartsWith("Warning:", StringComparison.Ordinal) &&
      entry.Contains("could not be deserialized", StringComparison.Ordinal));
    Assert.DoesNotContain(logger.Messages, entry =>
      entry.Contains("Successfully processed", StringComparison.Ordinal));
  }

  private sealed class CapturingLogger<T> : ILogger<T>
  {
    public List<string> Messages { get; } = [];

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
      LogLevel logLevel,
      TState state,
      Exception? exception,
      Func<TState, Exception?, string> formatter)
    {
      Messages.Add($"{logLevel}:{formatter(state, exception)}");
    }
  }
}

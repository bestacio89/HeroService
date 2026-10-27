using Franz.Common.Messaging.Messages;
using HeroService.Consumer.Processing;
using HeroService.Domain.Heroes.Versioned.GameVersion;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace HeroService.Testing.Consumer;

public sealed class GenericMessageProcessorTests
{
  private readonly GenericMessageProcessor<GameVersion> _processor =
    new(NullLogger<GenericMessageProcessor<GameVersion>>.Instance);

  [Fact]
  public void Process_WhenBodyIsMalformed_PropagatesDeserializationFailure()
  {
    var message = new Message { Body = "{" };

    Assert.Throws<JsonReaderException>(() => _processor.Process(message));
  }

  [Fact]
  public void Process_WhenBodyIsJsonNull_DoesNotTreatNullAsAnEntity()
  {
    var message = new Message { Body = "null" };

    var exception = Record.Exception(() => _processor.Process(message));

    Assert.Null(exception);
  }
}

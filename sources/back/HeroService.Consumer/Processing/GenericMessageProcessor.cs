using Franz.Common.Messaging;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Franz.Common.Messaging.Messages;

namespace HeroService.Consumer.Processing
{
  public class GenericMessageProcessor<T> : IGenericMessageProcessor<T> where T : class
  {
    private readonly ILogger<GenericMessageProcessor<T>> _logger;

    public GenericMessageProcessor(ILogger<GenericMessageProcessor<T>> logger)
    {
      _logger = logger;
    }

    public void Process(Message message)
    {
      try
      {
        _logger.LogInformation("Processing message for entity type {EntityType}.", typeof(T).Name);

        var entity = JsonConvert.DeserializeObject<T>(message.Body);
        if (entity == null)
        {
          _logger.LogWarning("Message body could not be deserialized to type {EntityType}.", typeof(T).Name);
          return;
        }

        ValidateEntity(entity);
        ProcessEntity(entity);

      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while processing message for entity type {EntityType}.", typeof(T).Name);
        throw;
      }
    }

    private void ValidateEntity(T entity)
    {
      _logger.LogDebug("Validating entity: {@Entity}", entity);
    }

    private void ProcessEntity(T entity)
    {
      _logger.LogWarning(
        "No persistence or business handler is configured for entity type {EntityType}; the payload was not applied.",
        typeof(T).Name);
    }
  }
}

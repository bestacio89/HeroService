using Franz.Common.Messaging.Messages;

namespace HeroService.Consumer.Processing
{
  public interface IGenericMessageProcessor<T> where T : class
  {
    void Process(Message message);
  }
}

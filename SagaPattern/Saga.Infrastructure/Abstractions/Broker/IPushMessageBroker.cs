namespace Saga.Infrastructure.Abstractions.Broker;

public interface IPushMessageBroker<in TMessage>
{
    void PushMessage(TMessage e);
}
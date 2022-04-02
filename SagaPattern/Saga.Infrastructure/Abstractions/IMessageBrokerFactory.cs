using System.Data;
using Saga.Infrastructure.Abstractions.Broker;

namespace Saga.Infrastructure.Abstractions;

public interface IMessageBrokerFactory
{
    IPullMessageBroker<TMessage> GetPullBroker<TMessage>(MessageType messageType, CommandType commandType);
        
    IPushMessageBroker<TMessage> GetPushBroker<TMessage>(MessageType messageType, CommandType commandType);
}
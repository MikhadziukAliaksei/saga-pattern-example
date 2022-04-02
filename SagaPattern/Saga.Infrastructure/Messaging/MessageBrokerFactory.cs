using System.Collections.Concurrent;
using Saga.Core.Commands;
using Saga.Infrastructure.Abstractions;
using Saga.Infrastructure.Abstractions.Broker;
using Saga.Infrastructure.Messaging.Broker;

namespace Saga.Infrastructure.Messaging;

public class MessageBrokerFactory : IMessageBrokerFactory
{
    private static readonly Dictionary<(Type, MessageType, CommandType), object> _queuePool = new();
    
    public IPullMessageBroker<TMessage> GetPullBroker<TMessage>(MessageType messageType, CommandType commandType)
    {
        return new PullMessageBroker<TMessage>(GetQueueFromPool<TMessage>(messageType, commandType));
    }

    public IPushMessageBroker<TMessage> GetPushBroker<TMessage>(MessageType messageType, CommandType commandType)
    {
        return new PushMessageBroker<TMessage>(GetQueueFromPool<TMessage>(messageType, commandType));
    }
    
    private static ConcurrentQueue<TMessage> GetQueueFromPool<TMessage>(MessageType messageType, CommandType commandType)
    {
        if (!_queuePool.ContainsKey((typeof(TMessage), messageType, commandType)))
        {
            _queuePool.Add((typeof(TMessage), messageType, commandType), new ConcurrentQueue<TMessage>());
        }
        return (ConcurrentQueue<TMessage>) _queuePool[(typeof(TMessage), messageType, commandType)];
    }
}
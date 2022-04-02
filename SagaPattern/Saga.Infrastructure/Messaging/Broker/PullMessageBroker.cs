using System.Collections.Concurrent;
using Saga.Infrastructure.Abstractions.Broker;

namespace Saga.Infrastructure.Messaging.Broker;

public class PullMessageBroker<TMessage> : IPullMessageBroker<TMessage>
{
    private readonly ConcurrentQueue<TMessage> _queue;

    public PullMessageBroker(ConcurrentQueue<TMessage> queue)
    {
        _queue = queue;
    }

    public event EventHandler<TMessage>? MessageReceived;
    
    public void Run()
    {
        var listener = new Thread(_ => StartWatchingQueue()) {IsBackground = true};
        listener.Start();
    }
    
    private void StartWatchingQueue()
    {
        while (true)
        {
            try
            {
                if (!_queue.IsEmpty)
                {
                    if (_queue.TryDequeue(out var message))
                    {
                        MessageReceived?.Invoke(this, message);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO
            }
            Thread.Sleep(100);
        }
    }
}
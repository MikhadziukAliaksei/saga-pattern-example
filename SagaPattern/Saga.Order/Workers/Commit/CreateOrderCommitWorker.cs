using Saga.Core.Commands;
using Saga.Core.Models.Errors;
using Saga.Infrastructure.Abstractions;
using Saga.Order.Interfaces;

namespace Saga.Order.Workers.Commit;

public class CreateOrderCommitWorker : ICreateOrderCommitWorker
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessageBrokerFactory _messageBrokerFactory;

    public CreateOrderCommitWorker(IOrderRepository orderRepository, IMessageBrokerFactory messageBrokerFactory)
    {
        _orderRepository = orderRepository;
        _messageBrokerFactory = messageBrokerFactory;
    }

    public void Run()
    {
        var inputBroker = _messageBrokerFactory.GetPullBroker<Core.Models.Order>(MessageType.Request, CommandType.Commit);
        var outputBroker = _messageBrokerFactory.GetPushBroker<Core.Models.Order>(MessageType.Success, CommandType.Commit);
        var errorBroker = _messageBrokerFactory.GetPushBroker<CreateOrderErrorInfo>(MessageType.Error, CommandType.Commit);
            
        inputBroker.MessageReceived += async (_, e) =>
        {
            try
            {
                await _orderRepository.CreateOrderAsync(e);
                outputBroker.PushMessage(e);
            }
            catch (Exception ex)
            {
                errorBroker.PushMessage(new CreateOrderErrorInfo()
                {
                    CorrelationId = e.CorrelationId,
                    Exception = ex
                });
            }
        };
            
        inputBroker.Run();
    }
}
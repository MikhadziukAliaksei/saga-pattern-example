using Saga.Core.Commands;
using Saga.Core.Models.Errors;
using Saga.Infrastructure.Abstractions;
using Saga.Order.Interfaces;

namespace Saga.Order.Workers.RollBack;

public class CreateOrderRollbackWorker : ICreateOrderRollbackWorker
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessageBrokerFactory _messageBrokerFactory;

    public CreateOrderRollbackWorker(IOrderRepository orderRepository, IMessageBrokerFactory messageBrokerFactory)
    {
        _orderRepository = orderRepository;
        _messageBrokerFactory = messageBrokerFactory;
    }

    public void Run()
    {
        var inputBroker = _messageBrokerFactory.GetPullBroker<Core.Models.Order>(MessageType.Request, CommandType.Rollback);
        var outputBroker = _messageBrokerFactory.GetPushBroker<Core.Models.Order>(MessageType.Success, CommandType.Rollback);
        var errorBroker = _messageBrokerFactory.GetPushBroker<CreateOrderErrorInfo>(MessageType.Error, CommandType.Rollback);
            
        inputBroker.MessageReceived += async (_, e) =>
        {
            try
            {
                await _orderRepository.DeleteOrderAsync(e);
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
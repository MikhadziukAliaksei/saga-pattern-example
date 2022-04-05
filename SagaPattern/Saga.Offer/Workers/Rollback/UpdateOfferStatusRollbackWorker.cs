using Saga.Core.Commands;
using Saga.Core.Models.Errors;
using Saga.Infrastructure.Abstractions;
using Saga.Offer.Interfaces;

namespace Saga.Offer.Workers.Rollback;

public class UpdateOfferStatusRollbackWorker : IUpdateOfferStatusRollbackWorker
{
    private readonly IOfferRepository _offerRepository;
    private readonly IMessageBrokerFactory _messageBrokerFactory;

    public UpdateOfferStatusRollbackWorker(IOfferRepository offerRepository, IMessageBrokerFactory messageBrokerFactory)
    {
        _offerRepository = offerRepository;
        _messageBrokerFactory = messageBrokerFactory;
    }

    public void Run()
    {
        var inputBroker = _messageBrokerFactory.GetPullBroker<Core.Models.Offer>(MessageType.Request, CommandType.Commit);
        var outputBroker = _messageBrokerFactory.GetPushBroker<Core.Models.Offer>(MessageType.Success, CommandType.Commit);
        var errorBroker = _messageBrokerFactory.GetPushBroker<UpdateOfferErrorInfo>(MessageType.Error, CommandType.Commit);
        
        inputBroker.MessageReceived += async (_, e) =>
        {
            try
            {
                e.Status = e.Status;
                await _offerRepository.UpdateOfferStatusAsync(e.TrackSubmissionId, e.Status);
                outputBroker.PushMessage(e);
            }
            catch (Exception ex)
            {
                errorBroker.PushMessage(new UpdateOfferErrorInfo()
                {
                    CorrelationId = e.CorrelationId,
                    Exception = ex
                });
            }
        };
        
        inputBroker.Run();
    }
}
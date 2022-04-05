using Saga.Core.Commands;
using Saga.Core.Models;
using Saga.Core.Models.Errors;
using Saga.Infrastructure.Abstractions;
using Saga.Submission.Interfaces;

namespace Saga.Submission.Workers.RollBack;

public class UpdateTrackSubmissionStatusRollbackWorker : IUpdateTrackSubmissionStatusRollbackWorker
{
    private readonly ITrackSubmissionRepository _trackSubmissionRepository;
    private readonly IMessageBrokerFactory _messageBrokerFactory;

    public UpdateTrackSubmissionStatusRollbackWorker(ITrackSubmissionRepository trackSubmissionRepository, IMessageBrokerFactory messageBrokerFactory)
    {
        _trackSubmissionRepository = trackSubmissionRepository;
        _messageBrokerFactory = messageBrokerFactory;
    }

    public void Run()
    {
        var inputBroker = _messageBrokerFactory.GetPullBroker<TrackSubmission>(MessageType.Request, CommandType.Rollback);
        var outputBroker = _messageBrokerFactory.GetPushBroker<TrackSubmission>(MessageType.Success, CommandType.Rollback);
        var errorBroker = _messageBrokerFactory.GetPushBroker<UpdateTrackSubmissionErrorInfo>(MessageType.Error, CommandType.Rollback);
        
        inputBroker.MessageReceived += async (_, e) =>
        {
            try
            {
                e.Status = e.Status;
                await _trackSubmissionRepository.UpdateTrackSubmissionStatusAsync(e.TrackSubmissionId, e.Status);
                outputBroker.PushMessage(e);
            }
            catch (Exception ex)
            {
                errorBroker.PushMessage(new UpdateTrackSubmissionErrorInfo
                {
                    CorrelationId = e.CorrelationId,
                    Exception = ex
                });
            }
            
            inputBroker.Run();
        };
    }
}
using Saga.Core.Commands;
using Saga.Core.Models;
using Saga.Core.Models.Errors;
using Saga.Infrastructure.Abstractions;
using Saga.Submission.Interfaces;

namespace Saga.Submission.Workers.Commit;

public class UpdateSubmissionStatusCommitWorker : IUpdateTrackSubmissionStatusCommitWorker
{
    private readonly ITrackSubmissionRepository _trackSubmissionRepository;
    private readonly IMessageBrokerFactory _messageBrokerFactory;

    public UpdateSubmissionStatusCommitWorker(ITrackSubmissionRepository trackSubmissionRepository, IMessageBrokerFactory messageBrokerFactory)
    {
        _trackSubmissionRepository = trackSubmissionRepository;
        _messageBrokerFactory = messageBrokerFactory;
    }

    public void Run()
    {
        var inputBroker = _messageBrokerFactory.GetPullBroker<TrackSubmission>(MessageType.Request, CommandType.Commit);
        var outputBroker = _messageBrokerFactory.GetPushBroker<TrackSubmission>(MessageType.Success, CommandType.Commit);
        var errorBroker = _messageBrokerFactory.GetPushBroker<UpdateTrackSubmissionErrorInfo>(MessageType.Error, CommandType.Commit);
        
        inputBroker.MessageReceived += async (_, e) =>
        {
            try
            {
                await _trackSubmissionRepository.UpdateTrackSubmissionStatusAsync(e.TrackSubmissionId, e.Status);
                outputBroker.PushMessage(e);
            }
            catch (Exception ex)
            {
                errorBroker.PushMessage(new UpdateTrackSubmissionErrorInfo()
                {
                    CorrelationId = e.CorrelationId,
                    Exception = ex
                });
            }
        };
        
        inputBroker.Run();
    }
}
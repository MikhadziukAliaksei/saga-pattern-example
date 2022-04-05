using Saga.Core.Commands;
using Saga.Core.Models;

namespace Saga.Orchistrator.Command;

public class UpdateTrackSubmissionRollbackCommand : Command<TrackSubmission>
{
    public override CommandType CommandType => CommandType.Rollback;
}
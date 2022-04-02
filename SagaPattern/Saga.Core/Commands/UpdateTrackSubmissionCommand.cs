using Saga.Core.Models;

namespace Saga.Core.Commands;

public class UpdateTrackSubmissionCommand : Command<TrackSubmission>
{
    public override CommandType CommandType => CommandType.Commit;
}
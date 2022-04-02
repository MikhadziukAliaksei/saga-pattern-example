using Saga.Core.Models;

namespace Saga.Submission.Interfaces;

public interface ITrackSubmissionRepository
{
    Task<TrackSubmission> UpdateTrackSubmissionStatusAsync( Guid id, TrackSubmissionStatus status);
}
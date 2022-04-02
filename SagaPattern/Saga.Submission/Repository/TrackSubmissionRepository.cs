using Saga.Core.Models;
using Saga.Submission.DataAccess.Contextes;
using Saga.Submission.Interfaces;

namespace Saga.Submission.Repository;

public class TrackSubmissionRepository : ITrackSubmissionRepository
{
    private readonly SubmissionDbContext _dbContext;

    public TrackSubmissionRepository(SubmissionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TrackSubmission> UpdateTrackSubmissionStatusAsync(Guid id, TrackSubmissionStatus status)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            var trackSub = _dbContext.TrackSubmissions.FirstOrDefault(x => x.TrackSubmissionId.Equals(id));

            trackSub.Status = status;

            _dbContext.TrackSubmissions.Update(trackSub);
            await _dbContext.SaveChangesAsync();    
            
            await transaction.CommitAsync();
            
            return trackSub;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
        
    }
}
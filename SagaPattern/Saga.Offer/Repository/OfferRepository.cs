using Saga.Core.Models;
using Saga.Offer.DataAccess.Contextes;
using Saga.Offer.Interfaces;

namespace Saga.Offer.Repository;

public class OfferRepository : IOfferRepository
{
    private readonly OfferDbContext _dbContext;

    public OfferRepository(OfferDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Core.Models.Offer> UpdateOfferStatusAsync(Guid id, OfferStatus status)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var offer = _dbContext.Offers.FirstOrDefault(x => x.TrackSubmissionId.Equals(id));

            offer.Status = status;

            _dbContext.Offers.Update(offer);
            await _dbContext.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return offer;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
}
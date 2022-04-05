using Saga.Core.Models;

namespace Saga.Offer.Interfaces;

public interface IOfferRepository
{
    Task<Core.Models.Offer> UpdateOfferStatusAsync( Guid id, OfferStatus status);
}
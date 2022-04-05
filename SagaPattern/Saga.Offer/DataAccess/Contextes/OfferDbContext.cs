using Microsoft.EntityFrameworkCore;

namespace Saga.Offer.DataAccess.Contextes;

public class OfferDbContext : DbContext
{
    public DbSet<Core.Models.Offer> Offers { get; set; }
        
    public OfferDbContext(DbContextOptions<OfferDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
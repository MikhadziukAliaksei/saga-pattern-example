using Microsoft.EntityFrameworkCore;
using Saga.Orchistrator.DTO;

namespace Saga.Orchistrator.DataAccess.Contexts;

public class SagaDbContext : DbContext
{
    public DbSet<SagaStateInfo> Sagas { get; set; }
    
    public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
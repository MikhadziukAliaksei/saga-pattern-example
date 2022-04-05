using Microsoft.EntityFrameworkCore;

namespace Saga.Order.DataAccess.Cpntexts;

public class OrderDbContext : DbContext
{
    public DbSet<Core.Models.Order> Orders { get; set; }

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
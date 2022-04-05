using Saga.Core.Models;
using Saga.Order.DataAccess.Cpntexts;
using Saga.Order.Interfaces;

namespace Saga.Order.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Core.Models.Order> CreateOrderAsync(Core.Models.Order order)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            order.OrderId = Guid.NewGuid();
            _dbContext.Orders.Add(order);
            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
            
            return order;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
       
    }

    public async Task DeleteOrderAsync(Core.Models.Order order)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();    
            
            await transaction.CommitAsync();
            
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
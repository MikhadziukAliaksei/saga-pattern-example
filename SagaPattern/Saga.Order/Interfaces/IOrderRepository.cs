namespace Saga.Order.Interfaces;

public interface IOrderRepository
{
    Task<Core.Models.Order>CreateOrderAsync(Core.Models.Order order);

    Task DeleteOrderAsync(Core.Models.Order order);
}
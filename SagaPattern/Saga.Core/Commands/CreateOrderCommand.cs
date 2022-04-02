using Saga.Core.Models;

namespace Saga.Core.Commands;

public class CreateOrderCommand : Command<Order>
{
    public override CommandType CommandType => CommandType.Commit;
}
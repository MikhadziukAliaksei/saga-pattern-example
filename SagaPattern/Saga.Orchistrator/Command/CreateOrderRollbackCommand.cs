using System.Diagnostics.CodeAnalysis;
using Saga.Core.Commands;
using Saga.Core.Models;

namespace Saga.Orchistrator.Command
{
    [ExcludeFromCodeCoverage]
    public sealed class CreateOrderRollbackCommand : Command<Order>
    {
        public override CommandType CommandType => CommandType.Rollback;
    }
}
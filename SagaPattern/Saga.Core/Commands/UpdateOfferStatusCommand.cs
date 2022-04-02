using Saga.Core.Models;

namespace Saga.Core.Commands;

public class UpdateOfferStatusCommand : Command<Offer>
{
    public override CommandType CommandType => CommandType.Commit;
}
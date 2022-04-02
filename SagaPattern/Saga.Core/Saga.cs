using Saga.Core.Commands;

namespace Saga.Core;

public class Saga
{
    public Guid CorrelationId { get; set; }

    public CreateOrderCommand CreateOrderCommand { get; init; }

    public UpdateOfferStatusCommand UpdateOfferStatusCommand { get; init; }

    public UpdateTrackSubmissionCommand UpdateTrackSubmissionCommand { get; init; }
}
using Microsoft.AspNetCore.Mvc;
using Saga.Core.Commands;
using Saga.Core.Models;

namespace GenerateModel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetModelRepositoryController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var correlationId = Guid.NewGuid();
            var trackSub = Guid.NewGuid();
            var model = new Saga.Core.Saga
            {
                CorrelationId = correlationId,
                CreateOrderCommand = new CreateOrderCommand
                {
                    Payload = new Order
                    {
                        CorrelationId = correlationId,
                        OrderId = correlationId,
                        ActivationDate = DateTime.UtcNow,
                        CompletionDate = null,
                        CompletionPercentage = 0,
                        CreationDate = DateTime.UtcNow,
                        Status = OrderStatus.Created,
                        UserId = Guid.NewGuid()
                    }
                    
                },
                UpdateTrackSubmissionCommand = new UpdateTrackSubmissionCommand
                {
                    Payload = new TrackSubmission
                    {
                        Budget = 100,
                        Budget2 = 200,
                        CorrelationId = correlationId,
                        Feedback = String.Empty,
                        Notes = String.Empty,
                        Status = TrackSubmissionStatus.Accepted,
                        TrackSubmissionId = trackSub,
                        Url = String.Empty
                    }
                },
                UpdateOfferStatusCommand = new UpdateOfferStatusCommand
                {
                    Payload = new Offer
                    {
                        CorrelationId = correlationId,
                        OfferId = Guid.NewGuid(),
                        CreationDate = DateTime.UtcNow,
                        Price = 500,
                        Status = OfferStatus.Created,
                        TimeEstimate = 4,
                        Streams = 50000,
                        TrackSubmissionId = trackSub,
                        TrackSubmission = new TrackSubmission
                        {
                            Budget = 100,
                            Budget2 = 200,
                            CorrelationId = correlationId,
                            Feedback = String.Empty,
                            Notes = String.Empty,
                            Status = TrackSubmissionStatus.Accepted,
                            TrackSubmissionId = trackSub,
                            Url = String.Empty
                        }
                    }
                }
            };
            return Ok(model);
        }
    }
}
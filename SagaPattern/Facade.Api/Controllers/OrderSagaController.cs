using Microsoft.AspNetCore.Mvc;
using Saga.Offer.Interfaces;
using Saga.Orchistrator;
using Saga.Order.Interfaces;
using Saga.Submission.Interfaces;

namespace Facade.Api.Controllers
{
    [ApiController]
    [Route("order-saga")]
    public class OrderSagaController : ControllerBase
    {
        private readonly IOrchestrator _orchestrator;
        private readonly ICreateOrderCommitWorker _createOrderWorker;
        private readonly ICreateOrderRollbackWorker _createOrderRollbackWorker;
        private readonly IUpdateTrackSubmissionStatusCommitWorker _trackSubmissionStatusCommitWorker;
        private readonly IUpdateTrackSubmissionStatusRollbackWorker _trackSubmissionStatusRollbackWorker;
        private readonly IUpdateOfferStatusCommitWorker _offerStatusCommitWorker;
        private readonly IUpdateOfferStatusRollbackWorker _offerStatusRollbackWorker;

        public OrderSagaController(IOrchestrator orchestrator, ICreateOrderCommitWorker createOrderWorker, ICreateOrderRollbackWorker createOrderRollbackWorker, IUpdateTrackSubmissionStatusCommitWorker trackSubmissionStatusCommitWorker, IUpdateTrackSubmissionStatusRollbackWorker trackSubmissionStatusRollbackWorker, IUpdateOfferStatusCommitWorker offerStatusCommitWorker, IUpdateOfferStatusRollbackWorker offerStatusRollbackWorker)
        {
            _orchestrator = orchestrator;
            _createOrderWorker = createOrderWorker;
            _createOrderRollbackWorker = createOrderRollbackWorker;
            _trackSubmissionStatusCommitWorker = trackSubmissionStatusCommitWorker;
            _trackSubmissionStatusRollbackWorker = trackSubmissionStatusRollbackWorker;
            _offerStatusCommitWorker = offerStatusCommitWorker;
            _offerStatusRollbackWorker = offerStatusRollbackWorker;
        }

        [HttpPost("run")]
        public async Task<IActionResult> Run(Saga.Core.Saga saga)
        {
            _createOrderWorker.Run();
            _trackSubmissionStatusCommitWorker.Run();
            _offerStatusCommitWorker.Run();
            _createOrderRollbackWorker.Run();
            _trackSubmissionStatusRollbackWorker.Run();
            _offerStatusRollbackWorker.Run();
            
            await _orchestrator.Run(saga);
            
            return await Task.FromResult(Accepted());
        }
    }
}
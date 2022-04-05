using Saga.Core.Commands;
using Saga.Core.Models;
using Saga.Core.Models.Errors;
using Saga.Infrastructure.Abstractions;
using Saga.Infrastructure.Abstractions.Broker;
using Saga.Orchistrator.Command;
using Saga.Orchistrator.DTO;
using Saga.Orchisttor.Repo;

namespace Saga.Orchistrator
{
    public sealed class Orchestrator : IOrchestrator
    {
        private Core.Saga _saga;
        private string _state;

        private readonly IMessageBrokerFactory _messageBrokerFactory;
        private readonly ISagaStateRepo _sagaStateRepo;
        private readonly Dictionary<(Type, CommandType), object> pushBrokers = new();

        public Orchestrator(IMessageBrokerFactory messageBrokerFactory, ISagaStateRepo sagaStateRepo)
        {
            _messageBrokerFactory = messageBrokerFactory;
            _sagaStateRepo = sagaStateRepo;

            pushBrokers.Add(
                (typeof(Order), CommandType.Commit),
                messageBrokerFactory.GetPushBroker<Order>(MessageType.Request, CommandType.Commit));
            pushBrokers.Add(
                (typeof(Order), CommandType.Rollback),
                messageBrokerFactory.GetPushBroker<Order>(MessageType.Request, CommandType.Rollback));

            pushBrokers.Add(
                (typeof(TrackSubmission), CommandType.Commit),
                messageBrokerFactory.GetPushBroker<TrackSubmission>(MessageType.Request, CommandType.Commit));
            pushBrokers.Add(
                (typeof(TrackSubmission), CommandType.Rollback),
                messageBrokerFactory.GetPushBroker<TrackSubmission>(MessageType.Request, CommandType.Rollback));

            pushBrokers.Add(
                (typeof(Offer), CommandType.Commit),
                messageBrokerFactory.GetPushBroker<Offer>(MessageType.Request, CommandType.Commit));
            pushBrokers.Add(
                (typeof(Offer), CommandType.Rollback),
                messageBrokerFactory.GetPushBroker<Offer>(MessageType.Request, CommandType.Rollback));
        }

        public async Task Run(Core.Saga saga)
        {
            RunCreateOrderBrokers();
            RunUpdateTrackSubmissionBrokers();
            RunUpdateOfferStatusBrokers();

            _saga = saga;
            _saga.CorrelationId = _saga.CorrelationId == default ? Guid.NewGuid() : _saga.CorrelationId;
            _state = SagaState.Begin;
            
            await RunNext();
        }

        private async Task RunNext()
        {
            void ExecuteCommand<TPayload>(Command<TPayload> cmd)
            {
                var broker = (IPushMessageBroker<TPayload>) pushBrokers[(typeof(TPayload), cmd.CommandType)];
                broker.PushMessage(cmd.Payload);
            }
            
            switch (_state)
            {
                case SagaState.Begin:
                    await UpdateStateAsync();
                    ExecuteCommand(_saga.CreateOrderCommand);
                    break;
                case SagaState.OrderCreated:
                    ExecuteCommand(_saga.UpdateTrackSubmissionCommand);
                    break;
                case SagaState.OrderCreationRollbackFailed:
                case SagaState.TrackSubmissionStatusUpdateRollbackFailed:
                case SagaState.OfferStatusUpdateRollbackFailed:
                    _state = SagaState.SagaUnexpectedError; 
                    // an additional "on-fail"-logic can be added here
                    await RunNext();
                    break;
                case SagaState.OrderCreationFailed:
                case SagaState.OrderCreationRollbackSucceed:
                    _state = SagaState.SagaFailed;  
                    // an additional "on-cancel"-logic can be added here
                    await RunNext();
                    break;
                case SagaState.TrackSubmissionStatusUpdated:
                    ExecuteCommand(_saga.UpdateOfferStatusCommand);
                    break;
                case SagaState.TrackSubmissionStatusUpdateFailed:
                case SagaState.TrackSubmissionStatusUpdateRollbackSucceed:
                    ExecuteCommand(new CreateOrderRollbackCommand
                    {
                        Payload = _saga.CreateOrderCommand.Payload
                    });
                    break;
                case SagaState.OfferStatusUpdated:
                    _state = SagaState.SagaSucceed;  
                    // an additional "on-success"-logic can be added here
                    await RunNext();
                    break;
                case SagaState.OfferStatusUpdateFailed:
                case SagaState.OfferStatusUpdateRollbackSucceed:
                    ExecuteCommand(new UpdateTrackSubmissionRollbackCommand()
                    {
                        Payload = _saga.UpdateTrackSubmissionCommand.Payload
                    });
                    break;
                case SagaState.SagaSucceed:
                case SagaState.SagaFailed:
                case SagaState.SagaUnexpectedError:
                    await UpdateStateAsync();
                    _state = SagaState.End;
                    await RunNext();
                    break;
                case SagaState.End:
                    await UpdateStateAsync();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task UpdateStateAsync(string info = null)
        {
            try
            {
                await _sagaStateRepo.UpdateStateAsync(new SagaStateInfo
                {
                    CorrelationId = _saga.CorrelationId,
                    TimeStamp = DateTimeOffset.UtcNow,
                    State = _state,
                    Info = info
                });
            }
            catch (Exception ex)
            {
                // an additional exception handling can be added here; we just skip it for simplicity
            } 
        }
        
        private void RunCreateOrderBrokers()
        {
            var successBroker =
                _messageBrokerFactory.GetPullBroker<Order>(MessageType.Success, CommandType.Commit);
            successBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OrderCreated;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successBroker.Run();

            var errorBroker = _messageBrokerFactory.GetPullBroker<CreateOrderErrorInfo>(MessageType.Error, CommandType.Commit);
            errorBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OrderCreationFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorBroker.Run();
                
            var successRollbackBroker =
                _messageBrokerFactory.GetPullBroker<Order>(MessageType.Success, CommandType.Rollback);
            successRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OrderCreationRollbackSucceed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successRollbackBroker.Run();

            var errorRollbackBroker = _messageBrokerFactory.GetPullBroker<CreateOrderErrorInfo>(MessageType.Error, CommandType.Rollback);
            errorRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OrderCreationRollbackFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorRollbackBroker.Run();
        }
        
        private void RunUpdateTrackSubmissionBrokers()
        {
            var successBroker =
                _messageBrokerFactory.GetPullBroker<TrackSubmission>(MessageType.Success, CommandType.Commit);
            successBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.TrackSubmissionStatusUpdated;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successBroker.Run();

            var errorBroker =
                _messageBrokerFactory.GetPullBroker<UpdateTrackSubmissionErrorInfo>(MessageType.Error, CommandType.Commit);
            errorBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.TrackSubmissionStatusUpdateFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorBroker.Run();
                
            var successRollbackBroker =
                _messageBrokerFactory.GetPullBroker<TrackSubmission>(MessageType.Success, CommandType.Rollback);
            successRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.TrackSubmissionStatusUpdateRollbackSucceed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successRollbackBroker.Run();

            var errorRollbackBroker =
                _messageBrokerFactory.GetPullBroker<UpdateTrackSubmissionErrorInfo>(MessageType.Error, CommandType.Rollback);
            errorRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.TrackSubmissionStatusUpdateRollbackFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorRollbackBroker.Run();
        }
        
        private void RunUpdateOfferStatusBrokers()
        {
            var successBroker = _messageBrokerFactory.GetPullBroker<Offer>(MessageType.Success, CommandType.Commit);
            successBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OfferStatusUpdated;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successBroker.Run();

            var errorBroker = _messageBrokerFactory.GetPullBroker<UpdateOfferErrorInfo>(MessageType.Error, CommandType.Commit);
            errorBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OfferStatusUpdateFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorBroker.Run();
                
            var successRollbackBroker = _messageBrokerFactory.GetPullBroker<Offer>(MessageType.Success, CommandType.Rollback);
            successRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.OfferStatusUpdateRollbackSucceed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            successRollbackBroker.Run();

            var errorRollbackBroker = _messageBrokerFactory.GetPullBroker<UpdateOfferErrorInfo>(MessageType.Error, CommandType.Rollback);
            errorRollbackBroker.MessageReceived += async (_, e) =>
            {
                _state = SagaState.TrackSubmissionStatusUpdateRollbackFailed;
                await UpdateStateAsync(e.ToJsonString());
                await RunNext();
            };
            errorRollbackBroker.Run();
        }
    }
}
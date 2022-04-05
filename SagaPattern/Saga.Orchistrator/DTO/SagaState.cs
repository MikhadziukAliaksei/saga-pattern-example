namespace Saga.Orchistrator.DTO
{
    public static class SagaState
    {
        public const string Begin = "Begin";
        public const string OrderCreated = "OrderCreated";
        public const string OrderCreationFailed = "OrderCreationFailed";
        public const string OrderCreationRollbackSucceed = "OrderCreationRollbackSucceed";
        public const string OrderCreationRollbackFailed = "OrderCreationRollbackFailed";
        public const string TrackSubmissionStatusUpdated = "TrackSubmissionStatusUpdated";
        public const string TrackSubmissionStatusUpdateFailed = "TrackSubmissionStatusUpdateFailed";
        public const string TrackSubmissionStatusUpdateRollbackSucceed = "TrackSubmissionStatusUpdateRollbackSucceed";
        public const string TrackSubmissionStatusUpdateRollbackFailed = "TrackSubmissionStatusUpdateRollbackFailed";
        public const string OfferStatusUpdated = "OfferStatusUpdated";
        public const string OfferStatusUpdateFailed = "OfferStatusUpdateFailed";
        public const string OfferStatusUpdateRollbackSucceed = "OfferStatusUpdateRollbackSucceed";
        public const string OfferStatusUpdateRollbackFailed = "OfferStatusUpdateRollbackFailed";
        public const string SagaSucceed = "SagaSucceed";
        public const string SagaFailed = "SagaFailed";
        public const string SagaUnexpectedError = "SagaUnexpectedError";
        public const string End = "End";
    }
}
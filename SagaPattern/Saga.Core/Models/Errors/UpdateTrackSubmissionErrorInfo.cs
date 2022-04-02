using Saga.Core.Extensions;

namespace Saga.Core.Models.Errors;

public class UpdateTrackSubmissionErrorInfo : ErrorInfo
{
    public string ToJsonString() => JsonExtensions.SerializeObject(this);
}
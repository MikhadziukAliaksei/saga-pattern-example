using Saga.Core.Extensions;

namespace Saga.Core.Models.Errors;

public class UpdateOfferErrorInfo : ErrorInfo
{
    public string ToJsonString() => JsonExtensions.SerializeObject(this);
}
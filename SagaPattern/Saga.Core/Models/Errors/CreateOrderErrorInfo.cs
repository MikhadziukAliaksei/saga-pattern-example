using Saga.Core.Extensions;

namespace Saga.Core.Models.Errors;

public class CreateOrderErrorInfo : ErrorInfo
{
    public string ToJsonString() => JsonExtensions.SerializeObject(this);
}
namespace Saga.Core.Models.Errors;

public class ErrorInfo : CommonInfo
{
    public Exception Exception { get; set; }
}
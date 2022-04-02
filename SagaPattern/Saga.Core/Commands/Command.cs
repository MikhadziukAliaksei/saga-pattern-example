namespace Saga.Core.Commands;

public abstract class Command<T> 
{
    public T Payload { get; set; }
    public  abstract CommandType CommandType { get; }
}
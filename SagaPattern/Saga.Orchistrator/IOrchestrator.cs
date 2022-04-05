namespace Saga.Orchistrator
{
    public interface IOrchestrator
    {
        Task Run(Core.Saga saga);
    }
}
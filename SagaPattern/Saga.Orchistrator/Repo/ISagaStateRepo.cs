using Saga.Orchistrator.DTO;

namespace Saga.Orchisttor.Repo
{
    public interface ISagaStateRepo
    {
        Task UpdateStateAsync(SagaStateInfo item);

        Task<List<SagaStateInfo>> GetListAsync(Guid correlation);
    }
}
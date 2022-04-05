using Saga.Orchistrator.DataAccess.Contexts;
using Saga.Orchistrator.DTO;
using Saga.Orchisttor.Repo;

namespace Saga.Orchistrator.Repo
{
    public sealed class SagaStateRepo : ISagaStateRepo
    {
        private readonly SagaDbContext _dbContext;

        public SagaStateRepo(SagaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task UpdateStateAsync(SagaStateInfo info)
        {
            try
            {
                _dbContext.Sagas.Add(info);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
        }

        public async Task<List<SagaStateInfo>> GetListAsync(Guid correlation)
        {
            var sagas = _dbContext.Sagas.Where(x => x.CorrelationId.Equals(correlation)).ToList();

            return sagas;
        }
    }
}
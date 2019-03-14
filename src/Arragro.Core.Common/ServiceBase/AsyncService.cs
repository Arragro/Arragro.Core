using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Repository;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.ServiceBase
{
    public abstract class AsyncService<TRepository, TModel> : BusinessRulesBase<TRepository, TModel>, IAsyncService<TModel> 
        where TModel : class
        where TRepository : IRepository<TModel>
    {
        public AsyncService(TRepository repository)
            : base(repository)
        {
        }

        public async Task<TModel> FindAsync(params object[] ids)
        {
            return await Repository.FindAsync(ids);
        }

        protected abstract Task ValidateModelRulesAsync(TModel model);

        protected abstract Task<TModel> InsertOrUpdateAsync(TModel model);

        public async Task ValidateModelAsync(TModel model)
        {
            ValidateModelPropertiesAndBuildRulesException(model);
            await ValidateModelRulesAsync(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public async Task<TModel> ValidateAndInsertOrUpdateAsync(TModel model)
        {
            await ValidateModelAsync(model);
            return await InsertOrUpdateAsync(model);
        }
    }
}
using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Arragro.Core.Common.ServiceBase
{
    public abstract class AsyncAuditableService<TRepository, TModel, TUserIdType> : AuditableBusinessRulesBase<TRepository, TModel, TUserIdType>, IAsyncAuditableService<TModel, TUserIdType> where TModel : class, IAuditable<TUserIdType>
        where TRepository : IRepository<TModel>
    {
        public AsyncAuditableService(TRepository repository)
            : base(repository)
        {
        }

        public async Task<TModel> FindAsync(params object[] ids)
        {
            return await Repository.FindAsync(ids);
        }

        protected abstract Task ValidateModelRulesAsync(TModel model, TUserIdType userId, params object[] otherValues);

        protected abstract Task<TModel> InsertOrUpdateAsync(TModel model, TUserIdType userId);

        public async Task ValidateModelAsync(TModel model, TUserIdType userId, params object[] otherValues)
        {
            RulesException.ErrorsForValidationResults(ValidateModelProperties(model));
            await ValidateModelRulesAsync(model, userId, otherValues);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public async Task<TModel> ValidateAndInsertOrUpdateAsync(TModel model, TUserIdType userId, params object[] otherValues)
        {
            await ValidateModelAsync(model, userId, otherValues);
            return await InsertOrUpdateAsync(model, userId);
        }
    }
}
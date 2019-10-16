using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Repository;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.ServiceBase
{
    public abstract class AuditableService<TRepository, TModel, TUserIdType> : AuditableBusinessRulesBase<TRepository, TModel, TUserIdType>, IAuditableService<TModel, TUserIdType> where TModel : class, IAuditable<TUserIdType>
        where TRepository : IRepository<TModel>
    {
        public AuditableService(TRepository repository)
            : base(repository)
        {
        }

        public TModel Find(params object[] ids)
        {
            return Repository.Find(ids);
        }

        public async Task<TModel> FindAsync(params object[] ids)
        {
            return await Repository.FindAsync(ids);
        }

        protected abstract void ValidateModelRules(TModel model);

        protected abstract TModel InsertOrUpdate(TModel model, TUserIdType userId);

        public void ValidateModel(TModel model)
        {
            RulesException.ErrorsForValidationResults(ValidateModelProperties(model));
            ValidateModelRules(model);

            RulesException.ThrowException();
        }

        public TModel ValidateAndInsertOrUpdate(TModel model, TUserIdType userId)
        {
            ValidateModel(model);
            return InsertOrUpdate(model, userId);
        }
    }
}
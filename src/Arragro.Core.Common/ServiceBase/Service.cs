using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Repository;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.ServiceBase
{
    public abstract class Service<TRepository, TModel> : BusinessRulesBase<TRepository, TModel>, IService<TModel> 
        where TModel : class
        where TRepository : IRepository<TModel>
    {
        public Service(TRepository repository)
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

        protected abstract void ValidateModelRules(TModel model, params object[] otherValues);

        protected abstract TModel InsertOrUpdate(TModel model);

        public void ValidateModel(TModel model, params object[] otherValues)
        {
            ValidateModelPropertiesAndBuildRulesException(model);
            ValidateModelRules(model, otherValues);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public TModel ValidateAndInsertOrUpdate(TModel model, params object[] otherValues)
        {
            ValidateModel(model, otherValues);
            return InsertOrUpdate(model);
        }
    }
}
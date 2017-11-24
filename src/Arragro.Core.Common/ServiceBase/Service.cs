using Arragro.Core.Common.BusinessRules;
using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Repository;
using System.Linq;

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

        protected abstract void ValidateModelRules(TModel model);

        protected abstract TModel InsertOrUpdate(TModel model);

        public void ValidateModel(TModel model)
        {
            ValidateModelPropertiesAndBuildRulesException(model);
            ValidateModelRules(model);

            if (RulesException.Errors.Any()) throw RulesException;
        }

        public TModel ValidateAndInsertOrUpdate(TModel model)
        {
            ValidateModel(model);
            return InsertOrUpdate(model);
        }
    }
}
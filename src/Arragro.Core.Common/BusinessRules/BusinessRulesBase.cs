using Arragro.Core.Common.Repository;
using Arragro.Core.Common.RulesExceptions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.BusinessRules
{
    public class BusinessRulesBase<TRepository, TModel> : RulesBase<TModel>
        where TModel : class
        where TRepository : IRepository<TModel>
    {
        protected readonly TRepository Repository;

        public BusinessRulesBase(TRepository repository) : base()
        {
            if (repository == null)
                throw new ArgumentNullException("repositoryBase");
            Repository = repository;
        }

        public int SaveChanges()
        {
            return Repository.SaveChanges();
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return Repository.SaveChanges(acceptAllChangesOnSuccess);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Repository.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
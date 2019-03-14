using Arragro.Core.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace Arragro.Core.EntityFrameworkCore
{
    public class BaseContext : DbContext, IBaseContext
    {
        public BaseContext(DbContextOptions options, QueryTrackingBehavior queryTrackingBehaviour = QueryTrackingBehavior.NoTracking) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = queryTrackingBehaviour;
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void SetModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await Database.BeginTransactionAsync();
        }

        public void CommitTransaction()
        {
            Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            Database.RollbackTransaction();
        }

        public IDbContextTransaction CurrentTransaction()
        {
            return Database.CurrentTransaction;
        }

        protected new void Dispose()
        {
            base.Dispose();
        }
    }
}

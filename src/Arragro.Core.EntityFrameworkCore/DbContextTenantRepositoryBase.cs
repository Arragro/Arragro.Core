using Arragro.Core.Common.Extentions;
using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Repository;
using Arragro.Core.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.EntityFrameworkCore
{
    public class DbContextTenantRepositoryBase<TEntity> :
        IDbContextRepositoryBase<TEntity>, 
        IRepository<TEntity> where TEntity : class, ITenantId
    {
        public IBaseContext BaseContext { get; private set; }
        protected TenantIdResolver TenantIdResolver { get; }

        public DbContextTenantRepositoryBase(
            IBaseContext baseContext,
            TenantIdResolver tenantIdResolver)
        {
            BaseContext = baseContext;
            TenantIdResolver = tenantIdResolver;
        }

        protected DbSet<TEntity> DbSet
        {
            get { return BaseContext.Set<TEntity>(); }
        }
        
        public TEntity Find(params object[] ids)
        {
            // Turn the HashTable of models into a Queryable
            var tenantId = TenantIdResolver.GetTenantId();
            var entity = DbSet.Find(ids);
            if (entity.TenantId != tenantId) return null;
            return entity;
        }

        public async Task<TEntity> FindAsync(params object[] ids)
        {
            // Turn the HashTable of models into a 
            var tenantId = TenantIdResolver.GetTenantId();
            var entity = await DbSet.FindAsync(ids);
            if (entity.TenantId != tenantId) return null;
            return entity;
        }

        public TEntity Delete(object[] ids)
        {
            var entity = Find(ids);
            return DbSet.Remove(entity).Entity;
        }

        public async Task<TEntity> DeleteAsync(params object[] ids)
        {
            var entity = await FindAsync(ids);
            return DbSet.Remove(entity).Entity;
        }

        public virtual IQueryable<TEntity> All()
        {
            return DbSet.Where(x => x.TenantId == TenantIdResolver.GetTenantId());
        }

        public virtual IQueryable<TEntity> AllNoTracking()
        {
            return DbSet.AsNoTracking().Where(x => x.TenantId == TenantIdResolver.GetTenantId());
        }

        public virtual IQueryable<TEntity> AllNoTracking(Expression<Func<TEntity, bool>> whereClause)
        {
            return DbSet.Where(x => x.TenantId == TenantIdResolver.GetTenantId()).Where(whereClause).AsNoTracking();
        }

        public TEntity InsertOrUpdate(TEntity model, bool add)
        {
            if (model.TenantId != TenantIdResolver.GetTenantId())
                throw new Exception("The entity you are trying to save has a different TenantId to the scope.");
            if (add)
            {
                return DbSet.Add(model).Entity;
            }
            else
            {
                BaseContext.SetModified(model);
                return model;
            }
        }

        private void SetAllEntryiesDetached()
        {
            foreach (EntityEntry entityEntry in BaseContext.ChangeTracker.Entries().ToArray())
            {
                if (entityEntry.Entity != null)
                {
                    entityEntry.State = EntityState.Detached;
                }
            }
        }

        private int SaveChanges(bool? acceptAllChangesOnSuccess)
        {
            try
            {
                int result;
                if (acceptAllChangesOnSuccess.HasValue)
                    result = BaseContext.SaveChanges(acceptAllChangesOnSuccess.Value);
                else
                    result = BaseContext.SaveChanges();

                SetAllEntryiesDetached();
                return result;
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException;
                throw;
            }
        }

        public int SaveChanges()
        {
            return SaveChanges(null);
        }

        public int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            return SaveChanges(acceptAllChangesOnSuccess);
        }
        
        private async Task<int> SaveChangesAsync(bool? acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                int result;
                if (acceptAllChangesOnSuccess.HasValue)
                    result = await BaseContext.SaveChangesAsync(acceptAllChangesOnSuccess.Value, cancellationToken);
                else
                    result = await BaseContext.SaveChangesAsync(cancellationToken);

                SetAllEntryiesDetached();
                return result;
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException;
                throw;
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(null, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public void Dispose()
        {
            BaseContext.Dispose();
        }
    }
}

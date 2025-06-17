using Arragro.Core.Common.Interfaces;
using Arragro.Core.Common.Repository;
using Arragro.Core.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.Core.EntityFrameworkCore
{
    public class DbContextTenantRepositoryAllIncludingBase<TEntity> :
        DbContextTenantRepositoryBase<TEntity>,
        Interfaces.IDbContextRepositoryAllIncludingBase<TEntity>,
        IRepository<TEntity> where TEntity : class, ITenantId
    {
        public DbContextTenantRepositoryAllIncludingBase(
            IBaseContext baseContext,
            ITenantIdResolver tenantIdResolver) : base(baseContext, tenantIdResolver) { }

        public virtual IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.Where(x => x.TenantId == TenantIdResolver.TenantId);
        }

        public virtual IQueryable<TEntity> AllIncludingNoTracking(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return AllIncluding(whereClause, includeProperties).AsNoTracking();
        }

        public virtual IQueryable<TEntity> AllIncludingNoTracking(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            return AllIncluding(includeProperties).AsNoTracking();
        }

        public virtual IQueryable<TEntity> AllIncluding(Expression<Func<TEntity, bool>> whereClause, params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet.Where(x => x.TenantId == TenantIdResolver.TenantId).Where(whereClause);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public new TEntity InsertOrUpdate(TEntity model, bool add)
        {
            if (model.TenantId != TenantIdResolver.TenantId)
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
    }
}

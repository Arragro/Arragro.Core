using Arragro.Core.Common.Repository;
using Arragro.Core.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.Core.EntityFrameworkCore
{
    public class DbContextRepositoryAllIncludingBase<TEntity> :
        DbContextRepositoryBase<TEntity>,
        Interfaces.IDbContextRepositoryAllIncludingBase<TEntity>,
        IRepository<TEntity> where TEntity : class
    {
        public DbContextRepositoryAllIncludingBase(IBaseContext baseContext) : base(baseContext) { }

        public virtual IQueryable<TEntity> AllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            IQueryable<TEntity> query = DbSet;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
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
            IQueryable<TEntity> query = DbSet.Where(whereClause);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public new TEntity InsertOrUpdate(TEntity model, bool add)
        {
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

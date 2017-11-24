using Arragro.Core.Common.Repository;
using Arragro.Core.EntityFrameworkCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Arragro.Core.EntityFrameworkCore
{
    public class DbContextRepositoryBase<TEntity> :
        IDbContextRepositoryBase<TEntity>, 
        IRepository<TEntity> where TEntity : class
    {
        private readonly IBaseContext _baseContext;

        public IBaseContext BaseContext { get { return _baseContext; } }

        public DbContextRepositoryBase(IBaseContext baseContext)
        {
            _baseContext = baseContext;
        }

        protected DbSet<TEntity> DbSet
        {
            get { return _baseContext.Set<TEntity>(); }
        }
        
        public TEntity Find(params object[] ids)
        {
            // Turn the HashTable of models into a Queryable
            return DbSet.Find(ids);
        }

        public TEntity Delete(params object[] ids)
        {
            var entity = Find(ids);
            return DbSet.Remove(entity).Entity;
        }

        public virtual IQueryable<TEntity> All()
        {
            return DbSet;
        }

        public virtual IQueryable<TEntity> AllNoTracking()
        {
            return DbSet.AsNoTracking();
        }

        public virtual IQueryable<TEntity> AllNoTracking(Expression<Func<TEntity, bool>> whereClause)
        {
            return DbSet.Where(whereClause).AsNoTracking();
        }

        public TEntity InsertOrUpdate(TEntity model, bool add)
        {
            if (add)
            {
                return DbSet.Add(model).Entity;
            }
            else
            {
                _baseContext.SetModified(model);
                return model;
            }
        }

        public int SaveChanges()
        {
            try
            {
                var result = _baseContext.SaveChanges();
                
                foreach (EntityEntry entityEntry in _baseContext.ChangeTracker.Entries().ToArray())
                {
                    if (entityEntry.Entity != null)
                    {
                        entityEntry.State = EntityState.Detached;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                var innerEx = ex.InnerException;
                throw;
            }
        }

        public void Dispose()
        {
            _baseContext.Dispose();
        }
    }
}

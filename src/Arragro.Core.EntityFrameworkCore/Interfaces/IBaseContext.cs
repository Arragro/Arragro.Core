using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace Arragro.Core.EntityFrameworkCore.Interfaces
{
    public interface IBaseContext : IDisposable
    {
        ChangeTracker ChangeTracker { get; }
        EntityEntry Entry(object entity);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        void SetModified(object entity);
        int SaveChanges();
    }
}

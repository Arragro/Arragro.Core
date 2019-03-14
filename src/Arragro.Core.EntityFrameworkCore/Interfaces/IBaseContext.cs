using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.EntityFrameworkCore.Interfaces
{
    public interface IBaseContext : IDisposable
    {
        ChangeTracker ChangeTracker { get; }
        EntityEntry Entry(object entity);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        void SetModified(object entity);
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void CommitTransaction();
        void RollbackTransaction();
        IDbContextTransaction CurrentTransaction();
    }
}

﻿using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface IService<TModel> where TModel : class
    {
        TModel Find(params object[] ids);
        Task<TModel> FindAsync(params object[] ids);
        TModel ValidateAndInsertOrUpdate(TModel model);
        void ValidateModel(TModel model);
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
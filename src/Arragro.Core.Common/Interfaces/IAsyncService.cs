﻿using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface IAsyncService<TModel> where TModel : class
    {
        Task<TModel> FindAsync(params object[] ids);
        Task<TModel> ValidateAndInsertOrUpdateAsync(TModel model);
        Task ValidateModelAsync(TModel model);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
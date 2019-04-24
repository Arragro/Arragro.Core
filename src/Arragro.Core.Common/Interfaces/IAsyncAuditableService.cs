using Arragro.Core.Common.BusinessRules;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface IAsyncAuditableService<TModel, TUserIdType> where TModel : class, IAuditable<TUserIdType>
    {
        Task<TModel> FindAsync(params object[] ids);
        Task<TModel> ValidateAndInsertOrUpdateAsync(TModel model, TUserIdType userId, params object[] otherValues);
        Task ValidateModelAsync(TModel model, TUserIdType userId, params object[] otherValues);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
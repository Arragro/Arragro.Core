using Arragro.Core.Common.BusinessRules;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface IAuditableService<TModel, TUserIdType> where TModel : class, IAuditable<TUserIdType>
    {
        TModel Find(params object[] ids);
        Task<TModel> FindAsync(params object[] ids);
        TModel ValidateAndInsertOrUpdate(TModel model, TUserIdType userId, params object[] otherValues);
        void ValidateModel(TModel model, TUserIdType userId, params object[] otherValues);
        int SaveChanges(bool acceptAllChangesOnSuccess);
        int SaveChanges();
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
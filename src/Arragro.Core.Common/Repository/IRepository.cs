using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Repository
{
    public interface IRepository<TModel> where TModel : class
    {
        TModel Find(params object[] ids);
        Task<TModel> FindAsync(params object[] ids);
        TModel Delete(params object[] ids);
        IQueryable<TModel> All();
        IQueryable<TModel> AllNoTracking();
        TModel InsertOrUpdate(TModel model, bool add);
        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
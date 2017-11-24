using System.Linq;

namespace Arragro.Core.Common.Repository
{
    public interface IRepository<TModel> where TModel : class
    {
        TModel Find(params object[] ids);
        TModel Delete(params object[] ids);
        IQueryable<TModel> All();
        IQueryable<TModel> AllNoTracking();
        TModel InsertOrUpdate(TModel model, bool add);
        int SaveChanges();
    }
}
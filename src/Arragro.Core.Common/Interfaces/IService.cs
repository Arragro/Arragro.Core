namespace Arragro.Core.Common.Interfaces
{
    public interface IService<TModel> where TModel : class
    {
        TModel Find(params object[] ids);
        TModel ValidateAndInsertOrUpdate(TModel model);
        void ValidateModel(TModel model);
        int SaveChanges();
    }
}
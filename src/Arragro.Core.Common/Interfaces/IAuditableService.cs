using Arragro.Core.Common.BusinessRules;

namespace Arragro.Core.Common.Interfaces
{
    public interface IAuditableService<TModel, TUserIdType> where TModel : class, IAuditable<TUserIdType>
    {
        TModel Find(params object[] ids);
        TModel ValidateAndInsertOrUpdate(TModel model, TUserIdType userId);
        void ValidateModel(TModel model);
        int SaveChanges();
    }
}
using System;
using System.Collections.Generic;

namespace Arragro.Core.Common.Interfaces
{
    internal interface IBaseService<TModel> where TModel : class
    {
        IEnumerable<TModel> SinceLastWrite(DateTime lastWriteDate);
        IEnumerable<TModel> All();
        bool Delete(TModel entity);
        TModel Find(params object[] ids);
        TModel Insert(TModel entity);
        bool Update(TModel entity);
        TModel ValidateAndInsert(TModel model);
        bool ValidateAndUpdate(TModel model);
        void ValidateModel(TModel model);
        IEnumerable<TModel> AllChanged(DateTime lastWriteDate);
    }
}

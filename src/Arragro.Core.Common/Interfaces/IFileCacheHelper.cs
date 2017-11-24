using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface IFileCacheHelper
    {
        Task SaveData<T>(IEnumerable<T> data, string applicationPath, bool loading = false) where T : class, IDictionaryKey;
        Task<IDictionary<string, T>> LoadSavedData<T>(string applicationPath) where T : class, IDictionaryKey;
    }
}

using System;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface IValidateUserAccess<TUserIdType>
    {
        Task<bool> UserHasAccessAsync(Guid id, TUserIdType userId);
        bool UserHasAccess(Guid id, TUserIdType userId);
    }
}

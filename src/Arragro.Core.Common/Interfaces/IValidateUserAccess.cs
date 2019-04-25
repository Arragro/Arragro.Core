using System;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface IValidateUserAccess<TUserIdType>
    {
        Task UserHasAccessAsync(Guid id, TUserIdType userId);
        void UserHasAccess(Guid id, TUserIdType userId);
    }
}

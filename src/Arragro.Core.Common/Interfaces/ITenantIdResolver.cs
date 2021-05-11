using System;
using System.Threading.Tasks;

namespace Arragro.Core.Common.Interfaces
{
    public interface ITenantIdResolver
    {
        Task<Guid> ResolveTenantIdAsync();
        Guid ResolveTenantId();
    }
}

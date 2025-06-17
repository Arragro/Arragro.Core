using System;

namespace Arragro.Core.Common.Interfaces
{
    public interface ITenantIdResolver
    {
        Guid? TenantId { get; set; }
    }
}

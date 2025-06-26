using Arragro.Core.Common.Interfaces;
using System;

namespace Arragro.Core.Common.Models
{
    public class TenantIdResolver : ITenantIdResolver
    {
        public Guid? TenantId { get; private set; } = null;

        public void SetTenantId(Guid tenantId)
        {
            TenantId = tenantId;
        }
    }
}

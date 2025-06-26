using Arragro.Core.Common.Interfaces;
using System;

namespace Arragro.Core.Common.BusinessRules
{
    public class TenantIdBase : ITenantId
    {
        public Guid TenantId { get; set; }
    }

    public class AuditableTenantIdBase<TUserIdType> : Auditable<TUserIdType>, IAuditable<TUserIdType>, ITenantId
    {
        public Guid TenantId { get; set; }
    }
}

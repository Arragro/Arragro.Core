using Arragro.Core.Common.Extentions;
using Arragro.Core.Common.Interfaces;
using System;

namespace Arragro.Core.Common.Models
{
    public class TenantIdResolver : ITenantIdResolver
    {
        public Guid? TenantId { get; set; } = null;
    }
}

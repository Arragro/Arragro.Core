using System;

namespace Arragro.Core.Web.Auth.Hmac.Models
{
    public class HmacApplicationSettings
    {
        public Guid AppId { get; set; }
        public string ValidationKey { get; set; }
    }
}

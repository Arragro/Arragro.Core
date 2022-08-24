using System;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.Core.Fastly
{
    public class FastlyApiTokens
    {
        public string ServiceIdsString { get; set; } = null;
        internal List<string> ServiceIds { get; set; } = new List<string>();

        public string ApiTokensString { get; set; } = null;
        internal List<string> ApiTokens { get; set; } = new List<string>();

        internal List<string> GetServiceIds()
        {
            if (ServiceIds.Count == 0)
            {
                if (!string.IsNullOrEmpty(ServiceIdsString))
                {
                    ServiceIds = ServiceIdsString.Split(',').ToList();
                }
            }
            return ServiceIds;
        }

        internal List<string> GetApiTokens()
        {
            if (ApiTokens.Count == 0)
            {
                if (!string.IsNullOrEmpty(ApiTokensString))
                {
                    ApiTokens = ApiTokensString.Split(',').ToList();
                }
            }
            return ApiTokens;
        }

        public bool Enabled
        {
            get
            {
                return !String.IsNullOrEmpty(ApiTokensString);
            }
        }
    }
}

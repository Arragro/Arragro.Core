using System.Collections.Generic;
using System.Linq;

namespace Arragro.Core.Fastly
{
    public class FastlyApiTokens
    {
        public string ServiceId { get; set; }
        public string ApiTokensString { get; set; } = null;
        public List<string> ApiTokens { get; set; } = new List<string>();

        public List<string> GetApiTokens()
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
    }
}

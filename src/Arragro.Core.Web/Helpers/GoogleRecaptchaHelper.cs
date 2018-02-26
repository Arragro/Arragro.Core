using Newtonsoft.Json;
using System.Collections.Generic;

namespace Arragro.Core.Web.Helpers
{
    public class GoogleRecaptcha
    {
        public static bool Validate(string encodedResponse, string googleSecret)
        {
            var client = new System.Net.WebClient();

            var googleReply = client.DownloadString($@"https://www.google.com/recaptcha/api/siteverify?secret={googleSecret}&response={encodedResponse}");

            var captchaResponse = JsonConvert.DeserializeObject<GoogleRecaptcha>(googleReply);

            return captchaResponse.Success;
        }

        private bool _success;
        [JsonProperty("success")]
        internal bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        private List<string> _errorCodes;
        [JsonProperty("error-codes")]
        internal List<string> ErrorCodes
        {
            get { return _errorCodes; }
            set { _errorCodes = value; }
        }
    }
}

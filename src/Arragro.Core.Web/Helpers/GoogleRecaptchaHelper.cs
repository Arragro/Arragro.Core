using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arragro.Core.Web.Helpers
{
    public class GoogleRecaptchaClient
    {
        private readonly HttpClient _httpClient;

        public GoogleRecaptchaClient(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public static void ConfigureGoogleRecaptcha(IServiceCollection services)
        {
            services.AddHttpClient<GoogleRecaptchaClient>(client =>
            {
                client.BaseAddress = new Uri("https://www.google.com");
            });
        }

        public async Task<bool> ValidateAsync(string encodedResponse, string googleSecret)
        {
            var captchaResponse = await ValidateAndReturnResponseAsync(encodedResponse, googleSecret);

            return captchaResponse.Success;
        }
        public async Task<GoogleRecaptcha> ValidateAndReturnResponseAsync(string encodedResponse, string googleSecret)
        {
            var googleResponse = await _httpClient.GetStringAsync($"/recaptcha/api/siteverify?secret={googleSecret}&response={encodedResponse}");

            var captchaResponse = JsonConvert.DeserializeObject<GoogleRecaptcha>(googleResponse);

            return captchaResponse;
        }
    }

    public class GoogleRecaptcha
    {
        private bool _success;
        [JsonProperty("success")]
        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        private List<string> _errorCodes;
        [JsonProperty("error-codes")]
        public List<string> ErrorCodes
        {
            get { return _errorCodes; }
            set { _errorCodes = value; }
        }

        private decimal _score;
        [JsonProperty("score")]
        public decimal Score
        {
            get { return _score; }
            set { _score = value; }
        }

        private string _action;
        [JsonProperty("action")]
        public string Action
        {
            get { return _action; }
            set { _action = value; }
        }

        private string _challengeTimestamp;
        [JsonProperty("challenge_ts")]
        public string ChallengeTimestamp
        {
            get { return _challengeTimestamp; }
            set { _challengeTimestamp = value; }
        }

        private string _hostname;
        [JsonProperty("hostname")]
        public string Hostname
        {
            get { return _hostname; }
            set { _hostname = value; }
        }
    }
}

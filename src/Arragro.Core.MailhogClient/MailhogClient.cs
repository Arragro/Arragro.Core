using Arragro.Core.MailhogClient.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arragro.Core.MailhogClient
{
    public class MailhogClient
    {
        private readonly HttpClient _httpClient;

        public MailhogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5080");
        }

        public async Task<Messages> GetMessagesAsync()
        {
            var httpResponse = await _httpClient.GetAsync($"/api/v2/messages");

            Messages messages = null;
            if (httpResponse.IsSuccessStatusCode)
            {
                var json = await httpResponse.Content.ReadAsStringAsync();
                messages = JsonConvert.DeserializeObject<Messages>(json);
            }

            return messages;
        }

        public async Task<Message> GetMessageAsync(string messageId)
        {
            var httpResponse = await _httpClient.GetAsync($"/api/v1/messages/{messageId}");

            Message message = null;
            if (httpResponse.IsSuccessStatusCode)
            {
                var json = await httpResponse.Content.ReadAsStringAsync();
                message = JsonConvert.DeserializeObject<Message>(json);
            }

            return message;
        }

        public async Task<bool> DeleteMessageAsync(string messageId)
        {
            var httpResponse = await _httpClient.DeleteAsync($"/api/v1/messages/{messageId}");

            return httpResponse.IsSuccessStatusCode;
        }
    }
}

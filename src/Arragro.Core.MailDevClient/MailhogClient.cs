using Arragro.Core.Common.Interfaces;
using Arragro.Core.MailhogClient.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Arragro.Core.MailDevClient
{
    public class MailDevClient : IEmailClient<Message>
    {
        private readonly HttpClient _httpClient;

        public MailDevClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5080");
        }

        public async Task<IEnumerable<Message>> GetMessagesAsync()
        {
            var httpResponse = await _httpClient.GetAsync($"/email");

            IEnumerable<Message> messages = null;
            if (httpResponse.IsSuccessStatusCode)
            {
                var json = await httpResponse.Content.ReadAsStringAsync();
                messages = JsonConvert.DeserializeObject<List<Message>>(json);
            }

            return messages;
        }

        public async Task<Message> GetMessageAsync(string messageId)
        {
            var httpResponse = await _httpClient.GetAsync($"/email/{messageId}");

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
            var httpResponse = await _httpClient.DeleteAsync($"/email/{messageId}");

            return httpResponse.IsSuccessStatusCode;
        }
    }
}

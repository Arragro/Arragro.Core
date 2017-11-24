using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace Arragro.Core.Common.Exceptions
{
    public class ApiHttpException : Exception
    {
        public HttpResponseMessage HttpResponseMessage { get; private set; }
        public readonly string HttpResponseMessageContent = null;

        public ApiHttpException(string message, HttpResponseMessage httpResponseMessage) : base(message)
        {
            HttpResponseMessage = httpResponseMessage;
            if (httpResponseMessage.Content.Headers.ContentLength > 0)
            {
                HttpResponseMessageContent = httpResponseMessage.Content.ReadAsStringAsync().Result;
            }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(GetType().FullName);
            stringBuilder.AppendLine("========================================");
            stringBuilder.AppendLine(Message);
            stringBuilder.AppendLine("========================================");
            stringBuilder.AppendLine($"Status Code: {HttpResponseMessage.StatusCode}");
            stringBuilder.AppendLine($"Reason Phrase: {HttpResponseMessage.ReasonPhrase}");
            stringBuilder.AppendLine($"Date: {HttpResponseMessage.Headers.Date}");
            if (HttpResponseMessageContent != null)
            {
                stringBuilder.AppendLine("========================================");
                stringBuilder.AppendLine($"Content: {HttpResponseMessageContent}");
                stringBuilder.AppendLine("========================================");
            }
            stringBuilder.AppendLine("========================================");
            stringBuilder.AppendLine("JSON Serialized Response:");
            stringBuilder.AppendLine("========================================");
            stringBuilder.AppendLine(JsonConvert.SerializeObject(HttpResponseMessage, Formatting.Indented));

            return stringBuilder.ToString();
        }
    }

    public class ApiHttpExceptionRetry: ApiHttpException
    {
        public ApiHttpExceptionRetry(string message, HttpResponseMessage httpResponseMessage) : base(message, httpResponseMessage)
        {
        }
    }
}

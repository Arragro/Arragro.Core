using System;
using System.Net.Http;

namespace Arragro.Core.Fastly
{
    public class FastlyException : Exception
    {
        public HttpResponseMessage HttpResponseMessage { get; private set; }
        public string Body { get; private set; }

        public FastlyException(string message, HttpResponseMessage httpResponseMessage, string body)
            : base(message)
        {
            HttpResponseMessage = httpResponseMessage;
            Body = body;
        }
    }
}

using Arragro.Core.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.Core.MailhogClient.Models
{
    public class Message
    {
        public string Id { get; set; }
        public MailhogEmailAddress From { get; set; }
        public List<MailhogEmailAddress> To { get; set; }
        public Content Content { get; set; }
        public Mime Mime { get; set; }
        public Raw Raw { get; set; }

        public ConsumableMessage ToConsumableMessage()
        {
            return new ConsumableMessage
            {
                From = new EmailAddress(Content.Headers["From"].First()),
                Tos = Content.Headers["To"].SelectMany(x => x.Split(',').Select(y => new EmailAddress(y))).ToList(),
                Headers = Content.Headers,
                Text = GetContent(ContentType.Text),
                Html = GetContent(ContentType.Html),
            };
        }

        private enum ContentType
        {
            Text,
            Html
        }

        private string GetContent(ContentType contentType)
        {
            foreach (var part in Mime.Parts)
            {
                if (part.Headers.ContainsKey("Content-Type"))
                {
                    string contentHeader = null;
                    foreach (var header in part.Headers)
                    {
                        if (part.Mime != null && part.Mime.Parts.Any() && header.Value.Any(x => x.StartsWith("multipart/alternative")))
                        {
                            foreach (var innerPart in part.Mime.Parts)
                            {
                                foreach (var innerHeader in innerPart.Headers)
                                {
                                    switch (contentType)
                                    {
                                        case ContentType.Html:
                                            contentHeader = innerHeader.Value.SingleOrDefault(x => x.StartsWith("text/html"));
                                            break;
                                        default:
                                            contentHeader = innerHeader.Value.SingleOrDefault(x => x.StartsWith("text/plain"));
                                            break;
                                    }
                                }
                                if (contentHeader != null)
                                    return innerPart.Body;
                            }
                        }

                        switch (contentType)
                        {
                            case ContentType.Html:
                                contentHeader = header.Value.SingleOrDefault(x => x.StartsWith("text/html"));
                                break;
                            default:
                                contentHeader = header.Value.SingleOrDefault(x => x.StartsWith("text/plain"));
                                break;
                        }
                    }
                    if (contentHeader != null)
                        return part.Body;
                }
            }
            return null;
        }
    }

    public class ConsumableMessage
    {
        public EmailAddress From { get; set; }
        public List<EmailAddress> Tos { get; set; }
        public Dictionary<string, List<string>> Headers { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
    }
}
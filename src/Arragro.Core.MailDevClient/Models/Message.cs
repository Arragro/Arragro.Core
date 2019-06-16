using Arragro.Core.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arragro.Core.MailhogClient.Models
{
    public class Message
    {
        public string Id { get; set; }
        public DateTime Time { get; set; }
        public List<MailDevEmailAddress> From { get; set; }
        public List<MailDevEmailAddress> To { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public bool Read { get; set; }
        public string MessageId { get; set; }
        public string Priority { get; set; }
        public List<Attachment> Attachments { get; set; }

        public ConsumableMessage ToConsumableMessage()
        {
            var from = From.First();
            return new ConsumableMessage
            {
                From = new EmailAddress(from.Address, from.Name),
                Tos = To.Select(x => new EmailAddress(x.Address, x.Name)).ToList(),
                Headers = Headers,
                Text = Text,
                Html = Html,
            };
        }
    }

    public class ConsumableMessage
    {
        public EmailAddress From { get; set; }
        public List<EmailAddress> Tos { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public string Text { get; set; }
        public string Html { get; set; }
    }
}
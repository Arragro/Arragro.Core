using System.Collections.Generic;
using System.IO;

namespace Arragro.Core.Common.Models
{
    public class EmailAttachment
    {
        public MemoryStream Stream { get; }
        public string MimeType { get; }

        public EmailAttachment(Stream stream, string mimeType)
        {
            MimeType = mimeType;
            stream.CopyTo(Stream);
            Stream.Position = 0;
        }
        public EmailAttachment(byte[] bytes, string mimeType)
        {
            MimeType = mimeType;
            Stream = new MemoryStream(bytes);
            Stream.Position = 0;
        }
    }
    public class EmailMessage
    {
        public string Subject { get; protected set; }
        public EmailAddress From { get; protected set; }
        public string Text { get; protected set; }
        public string Html { get; protected set; }
        public List<EmailAddress> Tos { get; } = new List<EmailAddress>();
        public List<EmailAddress> Ccs { get; } = new List<EmailAddress>();
        public List<EmailAddress> Bccs { get; } = new List<EmailAddress>();
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public Dictionary<string, EmailAttachment> Attachments = new Dictionary<string, EmailAttachment>();

        public EmailMessage(string subject, EmailAddress from, string text, string html, EmailAddress to)
        {
            Subject = subject;
            From = from;
            Text = text;
            Html = html;
            Tos.Add(to);
        }

        /// <summary>
        /// EmailMessage uses a from address of support@arragro.com
        /// </summary>
        public EmailMessage(string subject, string text, string html, EmailAddress to)
        {
            Subject = subject;
            From = new EmailAddress("support@arragro.com", "Arragro Support");
            Text = text;
            Html = html;
            Tos.Add(to);
        }
    }
}

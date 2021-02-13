using Arragro.Core.Common.Models;
using System;

namespace Arragro.Core.Email.Razor.Models
{
    public class EmailQueueModel : EmailMessage
    {
        public Guid ArragroId { get; set; }

        public EmailQueueModel(string subject, EmailAddress from, string text, string html, EmailAddress to)
            : base(subject, from, text, html, to)
        {
            ArragroId = Guid.NewGuid();
        }

        public EmailQueueModel(string subject, string text, string html, EmailAddress to)
            : base(subject, text, html, to)
        {
            ArragroId = Guid.NewGuid();
        }
    }
}

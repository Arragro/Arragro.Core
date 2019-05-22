using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmailAddress = Arragro.Core.Common.Models.EmailAddress;

namespace Arragro.Providers.SendgridEmailProvider
{
    public static class SendGridEmailAddressHelper
    {
        public static SendGrid.Helpers.Mail.EmailAddress ToSendGridEmailAddress(this EmailAddress emailAddress)
        {
            return new SendGrid.Helpers.Mail.EmailAddress(emailAddress.Email, emailAddress.Name);
        }
    }

    public class EmailProvider : IEmailProvider
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailProvider(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public async Task SendEmailAsync(string subject, string text, string html, EmailAddress from, List<EmailAddress> tos, List<EmailAddress> ccs = null, List<EmailAddress> bccs = null)
        {
            var client = new SendGridClient(_smtpSettings.SendgridApiKey);
            var fromEmailAddress = new EmailAddress(from.Email, from.Name);
            var message = new SendGridMessage();
            message.SetFrom(fromEmailAddress.ToSendGridEmailAddress());
            message.AddTos(tos.Select(x => x.ToSendGridEmailAddress()).ToList());
            if (ccs != null)
                message.AddCcs(ccs.Select(x => x.ToSendGridEmailAddress()).ToList());
            if (bccs != null)
                message.AddBccs(bccs.Select(x => x.ToSendGridEmailAddress()).ToList());
            message.Subject = subject;
            message.PlainTextContent = text;
            message.HtmlContent = html;
            var response = await client.SendEmailAsync(message);
        }

        public async Task SendEmailAsync(string subject, string text, string html, List<EmailAddress> tos, List<EmailAddress> ccs = null, List<EmailAddress> bccs = null)
        {
            await SendEmailAsync(subject, text, html, _smtpSettings.DefaultFrom, tos, ccs, bccs);
        }
    }
}

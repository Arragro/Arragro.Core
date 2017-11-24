using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Arragro.Providers.SendgridEmailProvider
{
    public class EmailProvider : IEmailProvider
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailProvider(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public void SendEmail(string subject, string text, string html, string from, string to)
        {
            var client = new SendGridClient(_smtpSettings.SendgridApiKey);
            var fromEmailAddress = new EmailAddress(from);
            var toEmailAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(fromEmailAddress, toEmailAddress, subject, text, html);
            var response = client.SendEmailAsync(msg).Result;
        }

        public void SendEmail(string subject, string text, string html, string to)
        {
            SendEmail(subject, text, html, _smtpSettings.DefaultFrom, to);
        }
    }
}

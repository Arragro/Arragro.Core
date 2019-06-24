using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
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

        public async Task<Guid> SendEmailAsync(EmailMessage emailMessage)
        {
            var client = new SendGridClient(_smtpSettings.SendgridApiKey);
            var message = new SendGridMessage();
            message.SetFrom(emailMessage.From.ToSendGridEmailAddress());
            message.AddTos(emailMessage.Tos.Select(x => x.ToSendGridEmailAddress()).ToList());
            if (emailMessage.Ccs.Any())
                message.AddCcs(emailMessage.Ccs.Select(x => x.ToSendGridEmailAddress()).ToList());
            if (emailMessage.Bccs.Any())
                message.AddBccs(emailMessage.Bccs.Select(x => x.ToSendGridEmailAddress()).ToList());

            message.Subject = emailMessage.Subject;
            message.PlainTextContent = emailMessage.Text;
            message.HtmlContent = emailMessage.Html;

            var arragroId = Guid.NewGuid();
            emailMessage.Headers.Add("arragro-id", arragroId.ToString());

            foreach (var key in emailMessage.Headers.Keys)
            {
                message.Headers.Add(key, emailMessage.Headers[key]);
            }

            foreach (var fileName in emailMessage.Attachments.Keys)
            {
                var emailAttachment = emailMessage.Attachments[fileName];
                var fileBytes = emailAttachment.Stream.ToArray();
                string base64Content = Convert.ToBase64String(fileBytes);
                message.AddAttachment(fileName, base64Content);
            }


            var response = await client.SendEmailAsync(message);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid responded with a {response.StatusCode} and the following message:\r\n\r\n{body}");
            }

            return arragroId;
        }
    }
}

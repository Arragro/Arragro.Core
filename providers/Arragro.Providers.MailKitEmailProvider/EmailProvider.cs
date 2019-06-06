using MimeKit;
using MailKit.Net.Smtp;
using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Arragro.Providers.MailKitEmailProvider
{
    public static class SendGridEmailAddressHelper
    {
        public static MailboxAddress ToMailboxAddress(this EmailAddress emailAddress)
        {
            return new MailboxAddress(emailAddress.Name, emailAddress.Email);
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
            var message = new MimeMessage();
            message.From.Add(emailMessage.From.ToMailboxAddress());
            emailMessage.Tos.ForEach(to => message.To.Add(to.ToMailboxAddress()));
            if (emailMessage.Ccs.Any())
                emailMessage.Ccs.ForEach(cc => message.To.Add(cc.ToMailboxAddress()));
            if (emailMessage.Bccs.Any())
            emailMessage.Bccs.ForEach(bcc => message.To.Add(bcc.ToMailboxAddress()));
            message.Subject = emailMessage.Subject;

            var bodyBuilder = new BodyBuilder ();
            bodyBuilder.HtmlBody = emailMessage.Html;
            bodyBuilder.TextBody = emailMessage.Text;

            message.Body = bodyBuilder.ToMessageBody ();

            var arragroId = Guid.NewGuid();
            emailMessage.Headers.Add("arragro-id", arragroId.ToString());

            foreach (var key in emailMessage.Headers.Keys)
            {
                message.Headers.Add(key, emailMessage.Headers[key]);
            }

            try
            {
                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    // client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect(_smtpSettings.Server, _smtpSettings.Port, false);

                    // Note: since we don't have an OAuth2 token, disable
                    // the XOAUTH2 authentication mechanism.
                    //client.AuthenticationMechanisms.Remove("XOAUTH2");

                    // Note: only needed if the SMTP server requires authentication
                    // client.Authenticate("user", "password");

                    await client.SendAsync(message).ConfigureAwait(true);
                    await client.DisconnectAsync(false).ConfigureAwait(true);
                }
            }
            catch (Exception ex) //todo add another try to send email
            {
                var e = ex;
                throw;
            }

            return arragroId;
        }
    }
}

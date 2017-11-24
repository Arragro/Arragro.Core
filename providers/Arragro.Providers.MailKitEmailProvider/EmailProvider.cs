using MimeKit;
using MailKit.Net.Smtp;
using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;

namespace Arragro.Providers.MailKitEmailProvider
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
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from));
            message.To.Add(new MailboxAddress(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder ();
            bodyBuilder.HtmlBody = html;
            bodyBuilder.TextBody = text;

            message.Body = bodyBuilder.ToMessageBody ();


            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                
                client.Connect(_smtpSettings.Server, _smtpSettings.Port, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                //client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("user", "password");

                client.Send(message);
                client.Disconnect(true);
            }
        }

        public void SendEmail(string subject, string text, string html, string to)
        {
           SendEmail(subject, text, html, _smtpSettings.DefaultFrom, to);
        }
    }
}

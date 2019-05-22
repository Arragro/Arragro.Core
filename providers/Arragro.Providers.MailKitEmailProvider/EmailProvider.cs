using MimeKit;
using MailKit.Net.Smtp;
using Arragro.Core.Common.Interfaces.Providers;
using Arragro.Core.Common.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

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

        public async Task SendEmailAsync(string subject, string text, string html, EmailAddress from, List<EmailAddress> tos, List<EmailAddress> ccs = null, List<EmailAddress> bccs = null)
        {
            var message = new MimeMessage();
            message.From.Add(from.ToMailboxAddress());
            if (tos == null)
                throw new ArgumentNullException("tos");
            tos.ForEach(to => message.To.Add(to.ToMailboxAddress()));
            if (ccs != null)
                ccs.ForEach(cc => message.To.Add(cc.ToMailboxAddress()));
            if (bccs != null)
                bccs.ForEach(bcc => message.To.Add(bcc.ToMailboxAddress()));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder ();
            bodyBuilder.HtmlBody = html;
            bodyBuilder.TextBody = text;

            message.Body = bodyBuilder.ToMessageBody ();

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
                    await client.DisconnectAsync(true).ConfigureAwait(true);
                }
            }
            catch (Exception ex) //todo add another try to send email
            {
                var e = ex;
                throw;
            }
        }

        public async Task SendEmailAsync(string subject, string text, string html, List<EmailAddress> tos, List<EmailAddress> ccs = null, List<EmailAddress> bccs = null)
        {
           await SendEmailAsync(subject, text, html, _smtpSettings.DefaultFrom, tos, ccs, bccs);
        }
    }
}

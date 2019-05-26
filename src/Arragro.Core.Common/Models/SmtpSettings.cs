using Arragro.Core.Common.Enums;

namespace Arragro.Core.Common.Models
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public EmailAddress DefaultFrom { get; set; }
        public string SendgridApiKey { get; set; }
    }

    public class EmailSettings : SmtpSettings
    {
        public EmailType EmailType { get; set; }

        public EmailSettings()
        {
            EmailType = EmailType.Smtp;
            Server = "localhost";
            Port = 25;
            DefaultFrom = new EmailAddress("support@arragro.com", "Arragro Support");
        }
    }
}

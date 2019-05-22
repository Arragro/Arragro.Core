namespace Arragro.Core.Common.Models
{
    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public EmailAddress DefaultFrom { get; set; }
        public string SendgridApiKey { get; set; }
    }
}

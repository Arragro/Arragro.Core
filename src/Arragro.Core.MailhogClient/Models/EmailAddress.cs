namespace Arragro.Core.MailhogClient.Models
{
    public class MailhogEmailAddress
    {
        public string Mailbox { get; set; }
        public string Domain { get; set; }

        public string ToEmailAddress()
        {
            return $"{Mailbox}@{Domain}";
        }
    }
}
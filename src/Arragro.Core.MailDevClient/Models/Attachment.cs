namespace Arragro.Core.MailhogClient.Models
{
    public class Attachment
    {
        public string ContentType { get; set; }
        public string ContentDisposition { get; set; }
        public string FileName { get; set; }
        public string GeneratedFileName { get; set; }
        public string ContentId { get; set; }
        public string Checksum { get; set; }
    }
}
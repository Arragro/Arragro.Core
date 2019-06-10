using System.Collections.Generic;

namespace Arragro.Core.MailhogClient.Models
{
    public class Part
    {
        public Dictionary<string, List<string>> Headers { get; set; }
        public string Body { get; set; }
        public int Size { get; set; }
        public string Mime { get; set; }
    }
}
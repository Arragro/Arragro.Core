using System.Collections.Generic;

namespace Arragro.Core.MailhogClient.Models
{
    public class Raw
    {
        public string From { get; set; }
        public List<string> To { get; set; }
        public string Data { get; set; }

    }
}
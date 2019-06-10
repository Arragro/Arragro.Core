using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.Core.MailhogClient.Models
{
    public class Messages
    {
        public int Total { get; set; }
        public int Count { get; set; }
        public int Start { get; set; }
        public List<Message> Items { get; set; }
    }
}

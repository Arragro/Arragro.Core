using System;

namespace Arragro.Core.Email.Razor.Models
{
    public class EmailQueueModel
    {
        public Guid ArragroId { get; set; }

        public EmailQueueModel()
        {
            ArragroId = Guid.NewGuid();
        }
    }
}

using System;

namespace Arragro.Core.Common.Models
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

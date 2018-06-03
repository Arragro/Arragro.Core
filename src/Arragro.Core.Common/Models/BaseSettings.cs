using System;
using System.Collections.Generic;
using System.Text;

namespace Arragro.Core.Common.Models
{
    public class WebInfoSettings
    {
        public bool IsWebInfoEnabled { get; set; }
        public Guid Secret { get; set; } = Guid.Empty;
    }

    public class BaseSettings
    {
        public WebInfoSettings WebInfoSettings { get; set; }
    }
}

using System.Collections.Generic;

namespace Arragro.Core.Common.Exceptions
{
    public class ApiErrorData
    {
        public string Message { get; set; }
        public Dictionary<string, IEnumerable<string>> ModelState { get; set; }

        public ApiErrorData()
        {
            ModelState = new Dictionary<string, IEnumerable<string>>();
        }
    }
}

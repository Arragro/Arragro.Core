using System;
using System.Linq;
using System.Text;

namespace Arragro.Core.Common.Exceptions
{
    public class ApiRulesException : Exception
    {
        public ApiErrorData ApiErrorData { get; set; }

        public ApiRulesException(ApiErrorData apiErrorData) : base(apiErrorData.Message)
        {
            ApiErrorData = apiErrorData;
        }

        public override string Message
        {
            get
            {
                var formattedModelState = new StringBuilder();
                foreach (var key in ApiErrorData.ModelState.Keys)
                {
                    if (ApiErrorData.ModelState[key].Any())
                    {
                        if (ApiErrorData.ModelState[key].Count() == 1)
                            if (!string.IsNullOrEmpty(key))
                                formattedModelState.AppendLine(string.Format("\t{0}: {1}", key, ApiErrorData.ModelState[key].ElementAt(0)));
                            else
                                formattedModelState.AppendLine(string.Format("\t{1}", key, ApiErrorData.ModelState[key].ElementAt(0)));
                        else
                            foreach (var error in ApiErrorData.ModelState[key])
                            {
                                formattedModelState.AppendLine(string.Format("\t\t{0}", error));
                            }
                    }
                }

                return string.Format("{0}\n\n{1}", base.Message, formattedModelState.ToString());
            }
        }
    }
}

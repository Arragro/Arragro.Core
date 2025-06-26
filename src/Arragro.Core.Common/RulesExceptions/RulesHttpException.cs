using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesHttpException<T> : Exception where T : class
    {
        public int Code { get; set; }
        public RulesExceptionDto<T> RulesExceptionDto { get; set; }

        // other fields
        protected RulesHttpException(int code)
        {
            Code = code;
        }

        public RulesHttpException(int code, string message) : base(message)
        {
            RulesExceptionDto = null;
            Code = code;
        }

        public RulesHttpException(int code, RulesExceptionDto<T> rulesExceptionDto)
        {
            Code = code;
            RulesExceptionDto = rulesExceptionDto;
        }

        public override string ToString()
        {
            var jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            if (RulesExceptionDto == null)
                return JsonConvert.SerializeObject(new { Code, ErrorMessages = new[] { base.Message }, Errors = new Dictionary<string, object>() }, jsonSerializerSettings);
            else
                return JsonConvert.SerializeObject(
                    new
                    {
                        Code,
                        Message = RulesExceptionDto.ToString(),
                        RulesExceptionDto.ErrorMessages,
                        RulesExceptionDto.Errors,
                        RulesExceptionDto.OtherData,
                        RulesExceptionDto.Data,
                    }, jsonSerializerSettings);
        }
    }
}

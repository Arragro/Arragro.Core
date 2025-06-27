using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;

namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesHttpException : Exception
    {
        public int Code { get; set; }
        public RulesExceptionDto RulesExceptionDto { get; set; }

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

        public RulesHttpException(int code, RulesExceptionDto rulesExceptionDto)
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
                        RulesExceptionDto.Data,
                    }, jsonSerializerSettings);
        }
    }

    public class RulesHttpException<T> : RulesHttpException where T : class
    {
        public RulesHttpException(int code, RulesExceptionWithDataDto<T> rulesExceptionDto) : base(code, rulesExceptionDto)
        {
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
                        (RulesExceptionDto as RulesExceptionWithDataDto<T>).OtherData,
                        RulesExceptionDto.Data,
                    }, jsonSerializerSettings);
        }
    }
}

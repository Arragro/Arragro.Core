namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesExceptionDto<T> : RulesExceptionDto where T : class
    {
        public T OtherData { get; set; }

        public RulesExceptionDto() { }

        public RulesExceptionDto(RulesExceptionDto rulesExceptionDto)
        {
            this.Data = rulesExceptionDto.Data;
            this.ErrorMessages = rulesExceptionDto.ErrorMessages;
            this.Errors = rulesExceptionDto.Errors;
            this.RulesExceptionListContainers = rulesExceptionDto.RulesExceptionListContainers;
        }
    }
}

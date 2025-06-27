namespace Arragro.Core.Common.RulesExceptions
{
    public class RulesExceptionWithDataDto<T> : RulesExceptionDto where T : class
    {
        public T OtherData { get; set; }

        public RulesExceptionWithDataDto() { }

        public RulesExceptionWithDataDto(RulesExceptionDto rulesExceptionDto)
        {
            this.Data = rulesExceptionDto.Data;
            this.ErrorMessages = rulesExceptionDto.ErrorMessages;
            this.Errors = rulesExceptionDto.Errors;
            this.RulesExceptionListContainers = rulesExceptionDto.RulesExceptionListContainers;
        }
    }
}

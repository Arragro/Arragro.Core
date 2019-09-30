using System;

namespace Arragro.Core.Common.BusinessRules
{
    public interface IAuditable<TUserIdType>
    {
        TUserIdType CreatedBy { get; set; }
        TUserIdType ModifiedBy { get; set; }
        DateTimeOffset CreatedDate { get; set; }
        DateTimeOffset ModifiedDate { get; set; }
    }
    
    public class Auditable<TUserIdType> : IAuditable<TUserIdType>
    {
        public TUserIdType CreatedBy { get; set; }
        public TUserIdType ModifiedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
    }
}

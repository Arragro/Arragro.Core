using System;

namespace Arragro.Core.Common.BusinessRules
{
    public interface IAuditable<TUserIdType>
    {
        TUserIdType CreatedBy { get; set; }
        TUserIdType ModifiedBy { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime ModifiedDate { get; set; }
    }
    
    public class Auditable<TUserIdType> : IAuditable<TUserIdType>
    {
        public TUserIdType CreatedBy { get; set; }
        public TUserIdType ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}

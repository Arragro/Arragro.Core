using Newtonsoft.Json;
using System;

namespace Arragro.Core.Common.Models
{
    public class QueueModel<TQueueEnumType>
          where TQueueEnumType : Enum
    {
        public TQueueEnumType EnumType { get; set; }
    }

    public class QueueModel<TQueueEnumType, TQueueModel> : QueueModel<TQueueEnumType>
        where TQueueModel : class, new()
        where TQueueEnumType : Enum
    {
        public string JsonData { get; set; }

        public TQueueModel GetQueueModel()
        {
            return JsonConvert.DeserializeObject<TQueueModel>(JsonData);
        }

        public static QueueModel<TQueueEnumType, TQueueModel> GetQueueModel(TQueueEnumType enumType, TQueueModel model)
        {
            return new QueueModel<TQueueEnumType, TQueueModel>
            {
                EnumType = enumType,
                JsonData = JsonConvert.SerializeObject(model)
            };
        }
    }
}

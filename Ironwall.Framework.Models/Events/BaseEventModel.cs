
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Mappers;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;

namespace Ironwall.Framework.Models.Events
{
    public abstract class BaseEventModel : BaseModel, IBaseEventModel
    {
        public BaseEventModel()
        {
            DateTime = DateTime.Now;
        }

        public BaseEventModel(IEventMapperBase model) : base(model.Id)
        {
            MessageType = (EnumEventType)model.MessageType;
            DateTime = DateTime.Parse(model.DateTime);
        }

        public BaseEventModel(IBaseEventMessageModel model) : base(model.Id)
        {
            MessageType = EnumHelper.GetEventType(model.Command);
            DateTime = model.DateTime;
        }

        protected BaseEventModel(IBaseEventModel model) : base(model.Id)
        {
            MessageType = model.MessageType;
            DateTime = model.DateTime;
        }

        [JsonProperty("type_event", Order = 5)]
        public EnumEventType? MessageType { get; set; }

        [JsonProperty("datetime", Order = 20)]
        public DateTime DateTime { get; set; }
    }
}

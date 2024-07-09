using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public abstract class EventMapperBase : BaseModel, IEventMapperBase
    {

        public EventMapperBase()
        {
            DateTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public EventMapperBase(IBaseEventModel model) : base(model.Id)
        {
            MessageType = (int)model.MessageType;
            DateTime = model.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }

        public EventMapperBase(IBaseEventMessageModel model) : base(model.Id)
        {
            MessageType = (int)EnumHelper.GetEventType(model.Command);
            DateTime = model.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ff");
        }
        [JsonProperty("type_event", Order = 2)]
        public int MessageType { get; set; }
        [JsonProperty("datetime", Order = 3)]
        public string DateTime { get; set; }
    }
}

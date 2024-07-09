using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using Ironwall.Redis.Message.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ConnectionRequestModel
        : BaseEventMessageModel, IConnectionRequestModel
    {
        public ConnectionRequestModel()
        {
            Command = EnumCmdType.EVENT_DETECTION_REQUEST;
        }


        public ConnectionRequestModel(IConnectionEventModel model)
            : base(EnumCmdType.EVENT_DETECTION_REQUEST)
        {
            Body = model as ConnectionEventModel;
        }
        [JsonProperty("body", Order = 6)]
        public ConnectionEventModel Body { get; set; }
    }
}

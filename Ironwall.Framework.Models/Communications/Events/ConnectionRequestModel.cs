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

        public ConnectionRequestModel(BrkConnection brk) : base(brk)
        {
            Command = EnumCmdType.EVENT_CONNECTION_REQUEST;

        }

        public ConnectionRequestModel(IConnectionEventModel model) : base(model)
        {
            Command = EnumCmdType.EVENT_CONNECTION_REQUEST;
        }

    }
}

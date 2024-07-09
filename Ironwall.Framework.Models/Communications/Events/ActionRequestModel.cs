using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ActionRequestModel
        : BaseEventMessageModel, IActionRequestModel
    {
        public ActionRequestModel()
        {
            Command = EnumCmdType.EVENT_ACTION_REQUEST;
        }

        public ActionRequestModel(IActionEventModel model)
            : base(EnumCmdType.EVENT_ACTION_REQUEST)
        {
            Body = model as ActionEventModel;
        }

        [JsonProperty("body", Order = 6)]
        public ActionEventModel Body { get; set; }

    }
}

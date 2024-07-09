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
    public class ActionResponseModel
    : ResponseModel, IActionResponseModel
    {
        public ActionResponseModel()
        {
            Command = EnumCmdType.EVENT_ACTION_RESPONSE;
        }

        public ActionResponseModel(bool success, string msg, IActionEventModel model = default)
            : base(EnumCmdType.EVENT_ACTION_RESPONSE, success, msg)
        {
            Command = EnumCmdType.EVENT_ACTION_RESPONSE;
            Body = model as ActionEventModel;
        }


        [JsonProperty("request_model", Order = 4)]
        public ActionEventModel Body { get; set; }

        

    }
}

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

        public ActionResponseModel(bool success, string msg, IActionRequestModel model = null)
            : base(success, msg)
        {
            Command = EnumCmdType.EVENT_ACTION_RESPONSE;
            RequestModel = model as ActionRequestModel;
        }


        [JsonProperty("request_model", Order = 4)]
        public ActionRequestModel RequestModel { get; set; }

        

    }
}

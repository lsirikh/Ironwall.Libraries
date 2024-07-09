using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ConnectionResponseModel
        : ResponseModel, IConnectionResponseModel
    {
        public ConnectionResponseModel()
        {

        }

        public ConnectionResponseModel(bool success, string msg, IConnectionRequestModel model = null)
            : base (success, msg)
        {
            Command = EnumCmdType.EVENT_CONNECTION_RESPONSE;
            RequestModel = model as ConnectionRequestModel;
        }

        [JsonProperty("request_model", Order = 4)]
        public ConnectionRequestModel RequestModel { get; set; }
    }
}

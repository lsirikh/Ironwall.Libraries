using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class MalfunctionResponseModel
    : ResponseModel, IMalfunctionResponseModel
    {
        public MalfunctionResponseModel()
        {

        }

        public MalfunctionResponseModel(bool success, string msg, IMalfunctionRequestModel model = null)
            : base(success, msg)
        {
            Command = (int)EnumCmdType.EVENT_MALFUNCTION_RESPONSE;
            RequestModel = model as MalfunctionRequestModel;
        }

        [JsonProperty("request_model", Order = 4)]
        public MalfunctionRequestModel RequestModel { get; set; }

    }
}

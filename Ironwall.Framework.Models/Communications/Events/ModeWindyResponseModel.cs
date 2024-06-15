using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class ModeWindyResponseModel
        : ResponseModel, IModeWindyResponseModel
    {
        public ModeWindyResponseModel()
        {

        }

        public ModeWindyResponseModel(bool success, string msg, IModeWindyRequestModel model)
            : base(success, msg)
        {
            Command = EnumCmdType.MODE_WINDY_RESPONSE;
            RequestModel = model as ModeWindyRequestModel;
        }

        [JsonProperty("request_model", Order = 4)]
        public ModeWindyRequestModel RequestModel { get; set; }
    }
}

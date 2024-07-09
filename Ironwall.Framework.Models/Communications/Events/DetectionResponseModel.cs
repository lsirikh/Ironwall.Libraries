using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Events
{
    public class DetectionResponseModel 
        : ResponseModel, IDetectionResponseModel
    {
        public DetectionResponseModel()
        {
            Command = EnumCmdType.EVENT_DETECTION_RESPONSE;
        }

        public DetectionResponseModel(bool success, string msg, IDetectionRequestModel model = null)
            : base(success, msg)
        {
            Command = EnumCmdType.EVENT_DETECTION_RESPONSE;
            
            RequestModel = model as DetectionRequestModel;
        }

        [JsonProperty("request_model", Order = 4)]
        public DetectionRequestModel RequestModel { get; set; }

    }
}

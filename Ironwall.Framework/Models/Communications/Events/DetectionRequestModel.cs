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
    public class DetectionRequestModel
        : BaseEventMessageModel, IDetectionRequestModel
    {
        public DetectionRequestModel()
        {
            Command = (int)EnumCmdType.EVENT_DETECTION_REQUEST;
        }

        /// <summary>
        /// Broker Message로 부터 Request Model을 생성
        /// </summary>
        /// <param name="brk">Broker Message</param>
        public DetectionRequestModel(BrkDectection brk) : base(brk)
        {
            Command = (int)EnumCmdType.EVENT_DETECTION_REQUEST;
            Detail = RequestFactory.Build<DetectionDetailModel>(brk.DetectionResult);
        }

        /// <summary>
        /// Event Model로 부터 Request Model을 생성
        /// </summary>
        /// <param name="model">Detection Event Model</param>
        public DetectionRequestModel(IDetectionEventModel model) : base(model)
        {
            Command = (int)EnumCmdType.EVENT_DETECTION_REQUEST;
            Detail = RequestFactory.Build<DetectionDetailModel>(model.Result);
        }


        [JsonProperty("detail", Order = 6)]
        public DetectionDetailModel Detail { get; set; }

        //public void Insert(string id, string group, int controller, int sensor, int uType, DetectionDetailModel detail, string dateTime)
        //{
        //    Insert(id, group, controller, sensor, uType, dateTime);
        //    Detail = detail;
        //}
    }
}

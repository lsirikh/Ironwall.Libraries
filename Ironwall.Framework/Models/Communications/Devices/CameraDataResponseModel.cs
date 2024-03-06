using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public class CameraDataResponseModel
        : ResponseModel, ICameraDataResponseModel
    {
        public CameraDataResponseModel()
        {
            Command = (int)EnumCmdType.CAMERA_DATA_RESPONSE;
            Cameras = new List<CameraDeviceModel>();
        }
        public CameraDataResponseModel(bool success, string content, List<CameraDeviceModel> cameras)
            : base(success, content)
        {
            Command = (int)EnumCmdType.CAMERA_DATA_RESPONSE;
            Cameras = cameras;
        }
        [JsonProperty("cameras", Order = 4)]
        public List<CameraDeviceModel> Cameras { get; private set; }
    }
}

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
    public class DeviceDataResponseModel
        : ResponseModel, IDeviceDataResponseModel
    {
        public DeviceDataResponseModel()
        {
            Command = EnumCmdType.DEVICE_DATA_RESPONSE;
        }

        public DeviceDataResponseModel(
            bool success
            , string content
            , List<ControllerDeviceModel> controllers
            , List<SensorDeviceModel> sensors
            , List<CameraDeviceModel> cameras)
            : base(success, content)
        {
            Command = EnumCmdType.DEVICE_DATA_RESPONSE;
            Controllers = controllers;
            Sensors = sensors;
            Cameras = cameras;
        }

        [JsonProperty("controllers", Order = 4)]
        public List<ControllerDeviceModel> Controllers { get; private set; }
        [JsonProperty("sensors", Order = 5)]
        public List<SensorDeviceModel> Sensors { get; private set; }
        [JsonProperty("cameras", Order = 6)]
        public List<CameraDeviceModel> Cameras { get; private set; }
    }
}

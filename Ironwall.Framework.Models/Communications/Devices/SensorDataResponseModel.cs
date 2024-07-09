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
    public class SensorDataResponseModel
        : ResponseModel, ISensorDataResponseModel
    {
        public SensorDataResponseModel()
        {
            Command = EnumCmdType.SENSOR_DATA_RESPONSE;
        }

        public SensorDataResponseModel(bool success, string content, List<SensorDeviceModel> sensors)
            : base(EnumCmdType.SENSOR_DATA_RESPONSE, success, content)
        {
            Body = sensors;
        }
        [JsonProperty("body", Order = 4)]
        public List<SensorDeviceModel> Body { get; private set; }
    }
}

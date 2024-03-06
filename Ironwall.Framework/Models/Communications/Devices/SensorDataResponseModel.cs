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
            Command = (int)EnumCmdType.SENSOR_DATA_RESPONSE;
            Sensors = new List<SensorDeviceModel>();
        }

        public SensorDataResponseModel(bool success, string content, List<SensorDeviceModel> sensors)
            : base(success, content)
        {
            Command = (int)EnumCmdType.SENSOR_DATA_RESPONSE;
            Sensors = sensors;
        }
        [JsonProperty("sensors", Order = 4)]
        public List<SensorDeviceModel> Sensors { get; private set; }
    }
}

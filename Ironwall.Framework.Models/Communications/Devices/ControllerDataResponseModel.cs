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
    public class ControllerDataResponseModel
        : ResponseModel, IControllerDataResponseModel
    {
        public ControllerDataResponseModel()
        {
            Command = EnumCmdType.CONTROLLER_DATA_RESPONSE;
        }

        public ControllerDataResponseModel(bool success, string content, List<ControllerDeviceModel> controller)
            : base(EnumCmdType.CONTROLLER_DATA_RESPONSE, success, content)
        {
            Body = controller;
        }

        [JsonProperty("body", Order = 4)]
        public List<ControllerDeviceModel> Body { get; private set; }
    }
}

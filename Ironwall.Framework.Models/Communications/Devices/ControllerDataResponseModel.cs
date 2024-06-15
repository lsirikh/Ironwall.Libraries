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
            : base(success, content)
        {
            Command = EnumCmdType.CONTROLLER_DATA_RESPONSE;
            Controllers = controller;
        }

        [JsonProperty("controllers", Order = 4)]
        public List<ControllerDeviceModel> Controllers { get; private set; }
    }
}

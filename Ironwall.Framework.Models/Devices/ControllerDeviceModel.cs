using Ironwall.Framework.Models.Communications.Devices;
using Ironwall.Framework.Models.Mappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Devices
{
    public class ControllerDeviceModel
        : BaseDeviceModel, IControllerDeviceModel
    {

        public ControllerDeviceModel()
        {

        }

        public ControllerDeviceModel(IControllerTableMapper model) : base(model)
        {
            IpAddress = model.IpAddress;
            Port = model.Port;
        }

        public ControllerDeviceModel(IControllerDeviceModel model) : base(model)
        {
            IpAddress = model.IpAddress;
            Port = model.Port;
        }

        //public ControllerDeviceModel(IControllerDeviceViewModel model) : base(model)
        //{
        //    IpAddress = model.IpAddress;
        //    Port = model.Port;
        //}

        [JsonProperty("ip_address", Order = 6)]
        public string IpAddress { get; set; }
        [JsonProperty("ip_port", Order = 7)]
        public int Port { get; set; }
    }
}

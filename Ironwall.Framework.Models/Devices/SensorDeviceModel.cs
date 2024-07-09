using Ironwall.Framework.Models.Mappers;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Devices
{
    public class SensorDeviceModel
        : BaseDeviceModel, ISensorDeviceModel
    {
        public SensorDeviceModel()
        {
            Controller = new ControllerDeviceModel();
        }
        public SensorDeviceModel(IDeviceMapperBase model) : base(model)
        {
        }

        public SensorDeviceModel(IDeviceMapperBase model, IControllerDeviceModel controller): base(model)
        {
            Controller = controller as ControllerDeviceModel;
        }

        [JsonProperty("controller", Order = 6)]
        public ControllerDeviceModel Controller { get; set; }
    }
}

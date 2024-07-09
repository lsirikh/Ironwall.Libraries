using Ironwall.Framework.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public class SensorTableMapper
        : DeviceMapperBase, ISensorTableMapper
    {

        public SensorTableMapper()
        {

        }

        public SensorTableMapper(ISensorDeviceModel model) : base(model)
        {
            Controller = model.Controller.Id;
        }
        public int Controller { get; set; }
    }
}

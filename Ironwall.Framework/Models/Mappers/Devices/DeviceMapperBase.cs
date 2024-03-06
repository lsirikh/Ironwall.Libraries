using Ironwall.Framework.Models.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public abstract class DeviceMapperBase 
        : IDeviceMapperBase

    {
        public DeviceMapperBase()
        {

        }

        public DeviceMapperBase(IBaseDeviceModel model)
        {
            DeviceId = model.Id;
            DeviceGroup = model.DeviceGroup;
            DeviceNumber = model.DeviceNumber;
            DeviceName = model.DeviceName;
            DeviceType = model.DeviceType;
            Version = model.Version;
            Status = model.Status;
        }
        public string DeviceId { get; set; }
        public int DeviceGroup { get; set; }
        public int DeviceNumber { get; set; }
        public string DeviceName { get; set; }
        public int DeviceType { get; set; }
        public string Version { get; set; }
        public int Status { get; set; }

    }
}

﻿using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Mappers
{
    public abstract class DeviceMapperBase : BaseModel, IDeviceMapperBase

    {
        public DeviceMapperBase()
        {

        }

        public DeviceMapperBase(IBaseDeviceModel model) : base(model)
        {
            DeviceGroup = model.DeviceGroup;
            DeviceNumber = model.DeviceNumber;
            DeviceName = model.DeviceName;
            DeviceType = model.DeviceType;
            Version = model.Version;
            Status = model.Status;
        }
        public int DeviceGroup { get; set; }
        public int DeviceNumber { get; set; }
        public string DeviceName { get; set; }
        public EnumDeviceType DeviceType { get; set; }
        public string Version { get; set; }
        public int Status { get; set; }

    }
}

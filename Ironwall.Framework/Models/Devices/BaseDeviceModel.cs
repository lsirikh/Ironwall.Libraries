using Ironwall.Framework.Models.Mappers;
using Ironwall.Framework.ViewModels.Devices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Framework.Models.Devices
{
    public class BaseDeviceModel : BaseModel, IBaseDeviceModel
    {
        public BaseDeviceModel()
        {

        }

        public BaseDeviceModel(IDeviceMapperBase model) : base(model.DeviceId)
        {
            Id = model.DeviceId;
            DeviceGroup = model.DeviceGroup;
            DeviceNumber = model.DeviceNumber;
            DeviceName = model.DeviceName;
            DeviceType = model.DeviceType;
            Version = model.Version;
            Status = model.Status;
        }

        public BaseDeviceModel(IBaseDeviceViewModel model) : base(model.Id)
        {
            Id = model.Id;
            DeviceGroup = model.DeviceGroup;
            DeviceNumber = model.DeviceNumber;
            DeviceName = model.DeviceName;
            DeviceType = model.DeviceType;
            Version = model.Version;
            Status = model.Status;
        }
        
        
        [JsonProperty("device_number", Order = 2)]
        public int DeviceGroup { get; set; }
        [JsonProperty("device_group", Order = 3)]
        public int DeviceNumber { get; set; }
        [JsonProperty("device_name", Order = 4)]
        public string DeviceName { get; set; }
        [JsonProperty("device_type", Order = 5)]
        public int DeviceType { get; set; }
        [JsonProperty("version", Order = 6)]
        public string Version { get; set; }
        
        /// <summary>
        /// 0: Normal
        /// 1: Error
        /// 2: Deactivate
        /// </summary>
        [JsonIgnore]
        public int Status { get; set; }
        
    }
}

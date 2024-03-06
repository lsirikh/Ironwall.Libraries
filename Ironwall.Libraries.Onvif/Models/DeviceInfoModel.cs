using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Onvif.Models
{
    public class DeviceInfoModel : IDeviceInfoModel
    {
        public DeviceInfoModel()
        {

        }
        
        public string FirmwareVersion { get; set; }
        public string HardwareId { get; set; }
        public string Manufacturer { get; set; }
        public string DeviceModel { get; set; }
        public string SerialNumber { get; set; }
    }
}

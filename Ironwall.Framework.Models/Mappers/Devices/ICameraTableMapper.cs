using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Mappers
{
    public interface ICameraTableMapper : IDeviceMapperBase
    {
        string IpAddress { get; set; }
        int Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        int Category { get; set; }
        string DeviceModel { get; set; }
        int RtspPort { get; set; }
        string RtspUri { get; set; }
        int Mode { get; set; }
    }
}
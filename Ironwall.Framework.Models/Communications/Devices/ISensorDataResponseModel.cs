using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ISensorDataResponseModel : IResponseModel
    {
        List<SensorDeviceModel> Body { get; }
    }
}
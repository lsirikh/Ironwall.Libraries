using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface IDeviceDataResponseModel : IResponseModel
    {
        List<CameraDeviceModel> Cameras { get; }
        List<ControllerDeviceModel> Controllers { get; }
        List<SensorDeviceModel> Sensors { get; }
    }
}
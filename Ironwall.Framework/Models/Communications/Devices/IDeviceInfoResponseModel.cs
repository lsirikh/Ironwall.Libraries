using Ironwall.Framework.Models.Devices;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface IDeviceInfoResponseModel : IResponseModel
    {
        DeviceDetailModel Detail { get; }
    }
}
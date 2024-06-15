
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.ViewModels.Devices
{
    public interface IBaseDeviceViewModel
    {
        int Id { get; set; }
        int DeviceGroup { get; set; }
        string DeviceName { get; set; }
        int DeviceNumber { get; set; }
        EnumDeviceType DeviceType { get; set; }
        int Status { get; set; }
        string Version { get; set; }
    }
}
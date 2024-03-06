
namespace Ironwall.Framework.ViewModels.Devices
{
    public interface IBaseDeviceViewModel
    {
        int DeviceGroup { get; set; }
        string DeviceName { get; set; }
        int DeviceNumber { get; set; }
        int DeviceType { get; set; }
        string Id { get; set; }
        int Status { get; set; }
        string Version { get; set; }
    }
}
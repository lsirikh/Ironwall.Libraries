namespace Ironwall.Framework.Models.Devices
{
    public interface IBaseDeviceModel : IBaseModel
    {
        int DeviceGroup { get; set; }
        string DeviceName { get; set; }
        int DeviceNumber { get; set; }
        int DeviceType { get; set; }
        string Version { get; set; }
        int Status { get; set; }
    }
}
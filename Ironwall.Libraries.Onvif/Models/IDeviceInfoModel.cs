namespace Ironwall.Libraries.Onvif.Models
{
    public interface IDeviceInfoModel
    {
        string DeviceModel { get; set; }
        string FirmwareVersion { get; set; }
        string HardwareId { get; set; }
        string Manufacturer { get; set; }
        string SerialNumber { get; set; }
    }
}
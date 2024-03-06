namespace Ironwall.Framework.Models.Mappers
{
    public interface IDeviceMapperBase
    {
        string DeviceId { get; set; }
        int DeviceGroup { get; set; }
        string DeviceName { get; set; }
        int DeviceNumber { get; set; }
        int DeviceType { get; set; }
        int Status { get; set; }
        string Version { get; set; }
    }
}
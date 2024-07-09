using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Mappers
{
    public interface IDeviceMapperBase : IBaseModel
    {
        int DeviceGroup { get; set; }
        int DeviceNumber { get; set; }
        string DeviceName { get; set; }
        EnumDeviceType DeviceType { get; set; }
        string Version { get; set; }
        EnumDeviceStatus Status { get; set; }
    }
}
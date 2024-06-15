using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Mappers
{
    public interface IDeviceMapperBase : IBaseModel
    {
        int DeviceGroup { get; set; }
        string DeviceName { get; set; }
        int DeviceNumber { get; set; }
        EnumDeviceType DeviceType { get; set; }
        int Status { get; set; }
        string Version { get; set; }
    }
}
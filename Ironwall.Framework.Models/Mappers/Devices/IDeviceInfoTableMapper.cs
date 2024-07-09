using Ironwall.Framework.Models.Mappers;

namespace Ironwall.Framework.Models
{
    public interface IDeviceInfoTableMapper : IUpdateMapperBase
    {
        int Camera { get; }
        int Controller { get; }
        int Sensor { get; }
    }
}
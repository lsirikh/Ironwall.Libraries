using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Events
{
    public interface IMetaEventModel : IBaseEventModel
    {
        string EventGroup { get; set; }
        EnumEventType? MessageType { get; set; }
        int Status { get; set; }
        IBaseDeviceModel Device { get; set; }
    }
}
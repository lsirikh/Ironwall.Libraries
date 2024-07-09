
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.ViewModels.Events
{
    public interface IMetaEventViewModel : IBaseEventViewModel
    {
        IBaseDeviceViewModel Device { get; set; }
        string EventGroup { get; set; }
        EnumEventType? MessageType { get; set; }
        EnumTrueFalse Status { get; set; }
    }
}
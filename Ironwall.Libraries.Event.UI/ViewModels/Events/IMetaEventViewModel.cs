using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels.Devices;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IMetaEventViewModel : IBaseEventViewModel<IMetaEventModel>
    {
        string EventGroup { get; set; }
        EnumEventType? MessageType { get; set; }
        IBaseDeviceViewModel Device { get; set; }
        EnumTrueFalse Status { get; set; }
    }
}
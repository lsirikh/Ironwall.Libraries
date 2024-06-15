using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Device.UI.ViewModels;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IMetaEventViewModel : IBaseEventViewModel<IMetaEventModel>
    {
        string EventGroup { get; set; }
        EnumEventType? MessageType { get; set; }
        DeviceViewModel Device { get; set; }
        int Status { get; set; }
    }
}
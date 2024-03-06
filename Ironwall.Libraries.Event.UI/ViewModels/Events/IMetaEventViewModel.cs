using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Device.UI.ViewModels;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IMetaEventViewModel : IBaseEventViewModel<IMetaEventModel>
    {
        string EventGroup { get; set; }
        int MessageType { get; set; }
        DeviceViewModel Device { get; set; }
        int Status { get; set; }
    }
}
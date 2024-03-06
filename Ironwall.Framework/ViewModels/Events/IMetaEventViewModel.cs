
using Ironwall.Framework.ViewModels.Devices;

namespace Ironwall.Framework.ViewModels.Events
{
    public interface IMetaEventViewModel : IBaseEventViewModel
    {
        IBaseDeviceViewModel Device { get; set; }
        string EventGroup { get; set; }
        int MessageType { get; set; }
        int Status { get; set; }
    }
}
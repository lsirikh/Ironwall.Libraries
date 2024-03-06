using Ironwall.Framework.Models.Devices;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    public interface ISensorDeviceViewModel : IDeviceViewModel
    {
        ControllerDeviceViewModel Controller { get; }
    }
}
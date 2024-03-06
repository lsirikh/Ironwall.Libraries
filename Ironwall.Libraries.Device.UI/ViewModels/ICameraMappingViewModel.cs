using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    public interface ICameraMappingViewModel : IBaseCustomViewModel<ICameraMappingModel>
    {
        string Group { get; set; }
        SensorDeviceViewModel Sensor { get; set; }
        CameraPresetViewModel FirstPreset { get; set; }
        CameraPresetViewModel SecondPreset { get; set; }
        
    }
}
using Ironwall.Framework.Models.Devices;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    public interface ICameraPresetViewModel : ICameraOptionViewModel
    {
        string PresetName { get; set; }
        bool IsHome { get; set; }
        double Pan { get; set; }
        double Tilt { get; set; }
        double Zoom { get; set; }
        int Delay { get; set; }
    }
}
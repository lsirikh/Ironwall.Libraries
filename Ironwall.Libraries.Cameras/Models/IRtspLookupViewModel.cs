using Ironwall.Libraries.Cameras.ViewModels;

namespace Ironwall.Libraries.Cameras.Models
{
    public interface IRtspLookupViewModel
    {
        CameraDeviceViewModel CameraDeviceViewModel { get; set; }
        CameraPresetViewModel CameraPresetViewModel { get; set; }
    }
}
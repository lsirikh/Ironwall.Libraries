using Ironwall.Libraries.RTSP.ViewModels;

namespace Ironwall.Libraries.RTSP.Models
{
    public interface IRtspLookupViewModel
    {
        CameraDeviceViewModel CameraDeviceViewModel { get; set; }
        CameraPresetViewModel CameraPresetViewModel { get; set; }
    }
}
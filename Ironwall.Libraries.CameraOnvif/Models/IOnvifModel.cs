using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.CameraOnvif.Providers;
using Ironwall.Libraries.CameraOnvif.Services;
using Ironwall.Libraries.Devices.Providers.Models;
using System.Threading;

namespace Ironwall.Libraries.CameraOnvif.Models
{
    public interface IOnvifModel
    {
        ICameraDeviceModel CameraDeviceModel { get; set; }
        OnvifControl OnvifControl { get; set; }
        PtzPresetProvider PtzPresetProvider { get; set; }
        CancellationTokenSource Cts { get; set; }
        bool IsMoving { get; set; }
    }
}
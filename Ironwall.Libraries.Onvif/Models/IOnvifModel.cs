using Ironwall.Libraries.Cameras.Models;
using Ironwall.Libraries.Onvif.DataProviders;
using OnvifControl.Libraries.Onvif.Services;
using System.Threading;

namespace Ironwall.Libraries.Onvif.Models
{
    public interface IOnvifModel
    {
        ICameraDeviceModel CameraDeviceModel { get; set; }
        CancellationTokenSource Cts { get; set; }
        IOnvifControlService OnvifControlService { get; set; }
        PTZPresetProvider PtzPresetProvider { get; set; }
        bool IsMoving { get; set; }
    }
}
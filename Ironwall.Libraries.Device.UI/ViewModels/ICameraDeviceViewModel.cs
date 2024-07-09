using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;
using System.Collections.Generic;

namespace Ironwall.Libraries.Device.UI.ViewModels
{
    public interface ICameraDeviceViewModel : IDeviceViewModel
    {
        string DeviceModel { get; set; }
        string IpAddress { get; set; }
        int Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        EnumCameraType Category { get; set; }
        List<CameraPresetModel> Presets { get; set; }
        List<CameraProfileModel> Profiles { get; set; }
        int RtspPort { get; set; }
        string RtspUri { get; set; }
        int Mode { get; set; }
    }
}
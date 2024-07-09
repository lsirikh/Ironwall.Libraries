using Ironwall.Libraries.Enums;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Devices
{
    public interface ICameraDeviceModel : IBaseDeviceModel
    {
        string IpAddress { get; set; }
        int Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        EnumCameraType Category { get; set; }
        List<CameraPresetModel> Presets { get; set; }
        List<CameraProfileModel> Profiles { get; set; }
        string DeviceModel { get; set; }
        string RtspUri { get; set; }
        int RtspPort { get; set; }
        int Mode { get; set; }
    }
}
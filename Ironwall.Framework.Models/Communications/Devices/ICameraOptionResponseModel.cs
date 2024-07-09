using Ironwall.Framework.Models.Devices;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraOptionResponseModel : IResponseModel
    {
        List<CameraPresetModel> Presets { get; }
        List<CameraProfileModel> Profiles { get; }
    }
}
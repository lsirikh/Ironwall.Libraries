using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraPresetResponseModel : IResponseModel
    {
        List<CameraPresetModel> List { get; set; }
    }
}
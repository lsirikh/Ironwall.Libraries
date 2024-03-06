using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraDataResponseModel
        : IResponseModel
    {
        List<CameraDeviceModel> Cameras { get; }
    }
}
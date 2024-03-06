using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraDataSaveRequestModel : IUserSessionBaseRequestModel
    {
        List<CameraDeviceModel> Cameras { get; set; }
    }
}
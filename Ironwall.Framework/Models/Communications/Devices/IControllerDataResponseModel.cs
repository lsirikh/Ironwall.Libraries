using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface IControllerDataResponseModel
        : IResponseModel
    {
        List<ControllerDeviceModel> Controllers { get; }
    }
}
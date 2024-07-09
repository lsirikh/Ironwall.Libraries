using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraMappingSaveResponseModel : IResponseModel
    {
        List<CameraMappingModel> Body { get; set; }
    }
}
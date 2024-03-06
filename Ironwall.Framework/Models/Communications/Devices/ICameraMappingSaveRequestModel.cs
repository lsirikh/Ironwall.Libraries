using Ironwall.Framework.Models.Devices;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.Devices
{
    public interface ICameraMappingSaveRequestModel : IUserSessionBaseRequestModel
    {
        List<CameraMappingModel> Mappings { get; set; }
    }
}
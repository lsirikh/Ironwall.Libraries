using Ironwall.Framework.Models.Vms;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    public interface IVmsApiMappingSaveResponseModel : IResponseModel
    {
        List<VmsMappingModel> Body { get; set; }
    }
}
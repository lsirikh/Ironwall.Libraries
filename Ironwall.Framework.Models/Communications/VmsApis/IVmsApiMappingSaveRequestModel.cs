using Ironwall.Framework.Models.Vms;
using System.Collections.Generic;

namespace Ironwall.Framework.Models.Communications.VmsApis
{
    public interface IVmsApiMappingSaveRequestModel : IBaseMessageModel
    {
        List<VmsMappingModel> Body { get; set; }
    }
}
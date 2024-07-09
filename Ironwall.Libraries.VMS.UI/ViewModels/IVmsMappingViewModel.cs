using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.VMS.UI.ViewModels
{
    public interface IVmsMappingViewModel : IBaseCustomViewModel<IVmsMappingModel>
    {
        int EventId { get; set; }
        int GroupNumber { get; set; }
        EnumTrueFalse Status { get; set; }
    }
}
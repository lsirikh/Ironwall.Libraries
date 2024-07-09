using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.VMS.UI.ViewModels
{
    public interface IVmsSensorViewModel: IBaseCustomViewModel<IVmsSensorModel>
    {
        BaseDeviceModel Device { get; set; }
        int GroupNumber { get; set; }
        EnumTrueFalse Status { get; set; }
    }
}
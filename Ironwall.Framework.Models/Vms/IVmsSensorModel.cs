using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Vms
{
    public interface IVmsSensorModel : IBaseModel
    {
        int GroupNumber { get; set; }
        BaseDeviceModel Device { get; set; }
        EnumTrueFalse Status { get; set; }
    }
}
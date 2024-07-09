using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Events
{
    public interface IMetaEventModel : IBaseEventModel
    {
        string EventGroup { get; set; }
        BaseDeviceModel Device { get; set; }
        EnumTrueFalse Status { get; set; }
    }
}
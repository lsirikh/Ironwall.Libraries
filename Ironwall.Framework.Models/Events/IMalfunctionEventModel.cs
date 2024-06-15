using Ironwall.Framework.Models.Devices;
using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.Models.Events
{
    public interface IMalfunctionEventModel : IMetaEventModel
    {
        int FirstEnd { get; set; }
        int FirstStart { get; set; }
        EnumFaultType Reason { get; set; }
        int SecondEnd { get; set; }
        int SecondStart { get; set; }
    }
}
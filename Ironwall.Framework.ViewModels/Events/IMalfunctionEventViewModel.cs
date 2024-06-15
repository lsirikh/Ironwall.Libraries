using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.ViewModels.Events
{
    public interface IMalfunctionEventViewModel : IMetaEventViewModel
    {
        int FirstEnd { get; set; }
        int FirstStart { get; set; }
        EnumFaultType Reason { get; set; }
        int SecondEnd { get; set; }
        int SecondStart { get; set; }
    }
}
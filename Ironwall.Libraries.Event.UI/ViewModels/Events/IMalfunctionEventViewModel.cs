using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IMalfunctionEventViewModel : IMetaEventViewModel
    {
        EnumFaultType Reason { get; set; }
        int FirstEnd { get; set; }
        int FirstStart { get; set; }
        int SecondEnd { get; set; }
        int SecondStart { get; set; }
    }
}
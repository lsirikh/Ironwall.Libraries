using Ironwall.Libraries.Enums;

namespace Ironwall.Framework.ViewModels.Events
{
    public interface IModeWindyEventViewModel : IBaseEventViewModel
    {
        EnumWindyMode ModeWindy { get; set; }
    }
}
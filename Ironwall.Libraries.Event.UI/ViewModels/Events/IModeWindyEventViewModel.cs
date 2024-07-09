using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IModeWindyEventViewModel : IBaseEventViewModel<IModeWindyEventModel>
    {
        EnumWindyMode ModeWindy { get; set; }
    }
}
using Ironwall.Framework.Models.Events;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IModeWindyEventViewModel : IBaseEventViewModel<IModeWindyEventModel>
    {
        int ModeWindy { get; set; }
    }
}
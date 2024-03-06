using Ironwall.Framework.Models.Events;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IActionEventViewModel : IBaseEventViewModel<IActionEventModel>
    {
        string Content { get; set; }
        MetaEventViewModel FromEvent { get; set; }
        string User { get; set; }
    }
}
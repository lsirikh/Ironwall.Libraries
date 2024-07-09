using Ironwall.Framework.Models.Events;

namespace Ironwall.Libraries.Event.UI.ViewModels.Events
{
    public interface IActionEventViewModel : IBaseEventViewModel<IActionEventModel>
    {
        IMetaEventViewModel FromEvent { get; set; }
        string Content { get; set; }
        string User { get; set; }
    }
}
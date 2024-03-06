using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public interface IExEventViewModel : IEventViewModel<IActionEventModel>
    {
        string Content { get; set; }
        string User { get; set; }
        int Status { get; set; }
        IMetaEventModel FromEventModel { get; set; }
    }
}
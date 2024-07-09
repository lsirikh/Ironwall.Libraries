using Caliburn.Micro;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public interface IPostEventViewModel : IExEventViewModel
    {
        IEventAggregator EventAggregator { get; set; }
    }
}
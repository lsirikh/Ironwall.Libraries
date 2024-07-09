using Ironwall.Framework.Models.Events;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public interface IExEventTimerViewModel<T> : IEventTimerViewModel<T> where T : IBaseEventModel
    {
    }
}
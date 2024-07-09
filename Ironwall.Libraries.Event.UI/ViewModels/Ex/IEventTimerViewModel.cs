using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public interface IEventTimerViewModel<T> : IExBaseEventViewModel<T> where T: IBaseEventModel
    {
        string Tag { get; set; }
        string TagFault { get; set; }
        int TimeDiscardSec { get; set; }
        CancellationTokenSource CancellationTokenSourceEvent { get; set; }
        event EventHandler EventHandlerTimer;
        void Cancel();
        Task ExecuteAsync(CancellationToken tokenSourceEvent = default);
        Task TaskFinal();
    }
}
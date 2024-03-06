using Caliburn.Micro;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using Ironwall.Libraries.Events.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public abstract class EventTimerViewModel<T> : BaseEventViewModel<T>, IEventTimerViewModel<T> where T : IBaseEventModel
    {
        #region - Ctors - 
        public EventTimerViewModel(T model) : base(model)
        {
        }
        #endregion

        #region - Abstracts - 
        public abstract Task TaskFinal();
        #endregion

        #region - Implemenations for IEventViewModel - 
        public async Task ExecuteAsync(CancellationToken tokenSourceEvent = default)
        {
            try
            {
                if (tokenSourceEvent.IsCancellationRequested)
                    tokenSourceEvent.ThrowIfCancellationRequested();

                var eventSetupModel = IoC.Get<EventSetupModel>();
                TimeSpan ts = (TimeDiscardSec < 0) ? Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(TimeDiscardSec);
                await Task.Delay(ts, tokenSourceEvent);
                await TaskFinal();

            }
            catch (OperationCanceledException ex)
            {
                await TaskFinal();
                Debug.WriteLine($"Raised OperationCanceledException in {nameof(ExecuteAsync)} of {nameof(EventTimerViewModel)}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(ExecuteAsync)} of {nameof(EventTimerViewModel)}: {ex.Message}");
            }
        }

        #endregion

        #region - Procedures -
        public void Cancel()
        {
            try
            {
                CancellationTokenSourceEvent?.Cancel();
                CancellationTokenSourceEvent?.Dispose();

                EventHandlerTimer?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EventTimerViewModel][Cancel]{ex.Message}");
            }
        }


        #endregion

        #region - Properties for IEventViewModel -

        //public T EventModel { get; set; }
        public CancellationTokenSource CancellationTokenSourceEvent { get; set; }
        #endregion

        #region - Properties -
        public string Tag { get; set; }
        public string TagFault { get; set; }
        public int TimeDiscardSec { get; set; }
        public event EventHandler EventHandlerTimer;
        #endregion
        #region - Attriibtes -
        #endregion
    }
}

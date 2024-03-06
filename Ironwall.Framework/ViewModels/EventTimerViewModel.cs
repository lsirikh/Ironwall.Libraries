using Ironwall.Framework.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Framework.ViewModels
{
    public abstract class EventTimerViewModel
        : NotifierPropertyChanged, IEventViewModel
    {
        #region - Ctors - 
        public EventTimerViewModel(IEventModel eventModel)
        {
            EventModel = eventModel;
        }
        #endregion

        #region - Abstracts - 
        public abstract Task TaskFinal();
        #endregion

        #region - Implemenations for IEventViewModel - 
        public async Task ExecuteAsync(CancellationToken tokenSourceEvent = default, CancellationTokenSource tokenSourceOunter = default)
        {
            try
            {
                if (tokenSourceEvent.IsCancellationRequested)
                    tokenSourceEvent.ThrowIfCancellationRequested();

                TimeSpan ts = (TimeDiscardSec < 0) ?
                            Timeout.InfiniteTimeSpan : TimeSpan.FromSeconds(TimeDiscardSec);
                
                await Task.Delay(ts, tokenSourceEvent);
                await TaskFinal();
            }
            catch (OperationCanceledException ex)
            {
                Debug.WriteLine($"Raised OperationCanceledException in {nameof(ExecuteAsync)} of {nameof(EventTimerViewModel)}: {ex.Message}");
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(ExecuteAsync)} of {nameof(EventTimerViewModel)}: {ex.Message}");
            }
            finally
            {
                if (tokenSourceOunter != null && !tokenSourceOunter.IsCancellationRequested)
                    tokenSourceOunter.Cancel();
            }
        }
        #endregion

        #region - Procedures -
        [Obsolete("Execute(int timeDiscardSec, CancellationTokenSource tokenSourceEvent = default) is deprecated, Don't use it.", true)]
        public async Task Execute(int timeDiscardSec, CancellationTokenSource tokenSourceEvent = default)
        {
            TimeDiscardSec = timeDiscardSec;
            CancellationTokenSourceEvent = tokenSourceEvent;
            await ExecuteAsync(CancellationTokenSourceEvent.Token);
        }

        public void Cancel()
        {
            try
            {
                if(!CancellationTokenSourceEvent.IsCancellationRequested)
                    CancellationTokenSourceEvent?.Cancel();
                CancellationTokenSourceEvent?.Dispose();

                EventHandlerTimer?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EventTimerViewModel][Cancel]{ex.Message}");
            }
        }

        // 이벤트 구독을 위한 메서드
        public void Subscribe(EventHandler handler)
        {
            if (EventHandlerTimer != null)
                EventHandlerTimer -= handler;

            EventHandlerTimer += handler;
            Debug.WriteLine($"ID({Id}) EventModel was subscribed!");
        }

        // 이벤트 구독 해제를 위한 메서드
        public void Unsubscribe(EventHandler handler)
        {
            if (EventHandlerTimer != null)
            {
                EventHandlerTimer -= handler;
                Debug.WriteLine($"ID({Id}) EventModel was discarded!");
            }
        }

        #endregion 

        #region - Properties for IEventViewModel -
        public int Id
        {
            get => EventModel.Id;
            set
            {
                EventModel.Id = value;
                OnPropertyChanged();
            }
        }

        public DateTime DateTime
        {
            get => EventModel.DateTime;
            set
            {
                EventModel.DateTime = value;
                OnPropertyChanged();
            }
        }

        public IEventModel EventModel { get; set; }
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

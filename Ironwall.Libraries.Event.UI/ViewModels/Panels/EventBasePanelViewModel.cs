using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Threading;
using System;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models;
using Ironwall.Libraries.Devices.Providers;
using System.Linq;
using System.Collections.Generic;
using Ironwall.Libraries.Enums;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using EventProvider = Ironwall.Libraries.Events.Providers.EventProvider;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.ViewModels;
using System.Collections.ObjectModel;
using Ironwall.Libraries.Event.UI.Models.Messages;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Event.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/21/2023 10:35:05 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class EventBasePanelViewModel<T> : BaseViewModel
    {

        #region - Ctors -
        public EventBasePanelViewModel(IEventAggregator eventAggregator
                                        , ILogService log)
                                        : base(eventAggregator, log)
        {
            ViewModelProvider = new ObservableCollection<T>();
        }
        #endregion
        #region - Implementation of Interface -
        protected abstract Task EventInitialize();
        protected abstract void EventClear();
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            StartDate = DateTimeHelper.GetCurrentTimeWithoutMS() - TimeSpan.FromDays(1);
            EndDate = DateTimeHelper.GetCurrentTimeWithoutMS();
            EndDateDisplay = StartDate;

            await EventInitialize().ConfigureAwait(false);

        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            EventClear();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyOfPropertyChange(() => StartDate);
                EndDateDisplay = _startDate;
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        public DateTime EndDateDisplay
        {
            get { return _endDateDisplay; }
            set
            {
                _endDateDisplay = value;
                NotifyOfPropertyChange(() => EndDateDisplay);
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }


        public int Total
        {
            get { return _total; }
            set
            {
                _total = value;
                NotifyOfPropertyChange(() => Total);
            }
        }

        public ObservableCollection<T> ViewModelProvider { get; set; }
        #endregion
        #region - Attributes -
        protected DateTime _startDate;
        protected DateTime _endDate;
        protected DateTime _endDateDisplay;
        protected int _total;
        protected bool _isVisible;
        #endregion
    }
}

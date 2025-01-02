using Caliburn.Micro;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.Events.Models;
using Ironwall.Libraries.Base.Services;
using System;

namespace Ironwall.Libraries.Event.UI.ViewModels
{
    public class BaseEventListPanelViewModel
        : Screen
    {
        #region - Ctors -
        public BaseEventListPanelViewModel()
        {
            _eventAggregator = IoC.Get<IEventAggregator>();
        }

        public BaseEventListPanelViewModel(
            IEventAggregator eventAggregator,
            ILogService log,
            EventSetupModel eventSetupModel)
        {
            _eventAggregator = eventAggregator;
            _log = log;
            _eventSetupModel = eventSetupModel;
            _class = this.GetType();
            _eventAggregator?.SubscribeOnUIThread(this);
        }
        ~BaseEventListPanelViewModel()
        {
            _eventAggregator?.Unsubscribe(this);
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string NameTabHeader { get; set; }

        public string NameKind
        {
            get { return _nameKind; }
            set 
            { 
                _nameKind = value;
                NotifyOfPropertyChange(() => NameKind);
            }
        }

        public CancellationTokenSource Cts { get; protected set; }
        #endregion
        #region - Attributes -
        private string _nameKind;
        protected IEventAggregator _eventAggregator;
        protected ILogService _log;
        protected EventSetupModel _eventSetupModel;
        protected Type _class;
        protected object _locker;
        #endregion
    }
}
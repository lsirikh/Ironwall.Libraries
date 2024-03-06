using Caliburn.Micro;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.Events.Models;

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
            EventSetupModel eventSetupModel)
        {
            _eventAggregator = eventAggregator;
            _eventSetupModel = eventSetupModel;

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
        protected EventSetupModel _eventSetupModel;
        protected object _locker;
        #endregion
    }
}
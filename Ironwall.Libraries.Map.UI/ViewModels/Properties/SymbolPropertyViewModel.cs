using Caliburn.Micro;
using Ironwall.Libraries.Map.UI.ViewModels.Symbols;
using System.Threading.Tasks;
using System.Threading;

namespace Ironwall.Libraries.Map.UI.ViewModels.Properties
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 4/28/2023 1:00:38 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SymbolPropertyViewModel 
        : Screen
        
    {

        #region - Ctors -
        public SymbolPropertyViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        #endregion
        #region - Implementation of Interface -
       
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            Model = new TextSymbolViewModel();
            _eventAggregator.SubscribeOnUIThread(this);
            return base.OnActivateAsync(cancellationToken);
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            _eventAggregator.Unsubscribe(this);
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
        public ISymbolViewModel Model
        {
            get { return _model; }
            set
            {
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        public bool IsEnable
        {
            get { return _isEnable; }
            set { _isEnable = value; 
                NotifyOfPropertyChange(() => IsEnable); }
        }
        #endregion
        #region - Attributes -
        private bool _isEnable;
        private ISymbolViewModel _model;
        private IEventAggregator _eventAggregator;
        #endregion
    }
}

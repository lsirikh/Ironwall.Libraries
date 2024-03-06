using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.Device.UI.Providers;
using Ironwall.Libraries.Devices.Providers;

namespace Ironwall.Libraries.Device.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/22/2023 5:52:31 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class ControllerPanelViewModel : BaseViewModel
    {

        #region - Ctors -
        public ControllerPanelViewModel(IEventAggregator eventAggregator)
            :base(eventAggregator)
        {

        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            _ = DataInitialize(cancellationToken);

            await Task.Delay(1000);
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            IsVisible = false;
            await base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private Task DataInitialize(CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(500);
                ViewModelProvider = IoC.Get<ControllerViewModelProvider>();
                NotifyOfPropertyChange(() => ViewModelProvider);
                IsVisible = true;
            });
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public ControllerViewModelProvider ViewModelProvider { get; private set; }
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }
        #endregion
        #region - Attributes -
        protected bool _isVisible;
        #endregion
    }
}

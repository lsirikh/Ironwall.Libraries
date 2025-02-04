﻿using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Device.UI.Providers;
using System.Threading.Tasks;
using System.Threading;
using Caliburn.Micro;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Device.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/22/2023 5:52:45 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SensorPanelViewModel : BaseViewModel
    {
        #region - Ctors -
        public SensorPanelViewModel(IEventAggregator eventAggregator
                                    , ILogService log)
                                    : base(eventAggregator, log)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            await DataInitialize(cancellationToken).ConfigureAwait(false);
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
        private async Task DataInitialize(CancellationToken cancellationToken = default)
        {
            await Task.Delay(200);
            ViewModelProvider = IoC.Get<SensorViewModelProvider>();
            NotifyOfPropertyChange(() => ViewModelProvider);
            IsVisible = true;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public SensorViewModelProvider ViewModelProvider { get; set; }
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

using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.UI.ViewModels.Panels.SetupPanels;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Windows.Controls;
using Ironwall.Libraries.VMS.UI.Views.Panels.SetupPanels;

namespace Ironwall.Libraries.VMS.UI.ViewModels.Panels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/12/2024 5:04:13 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsSetupPanelViewModel : ConductorOneViewModel
    {

        #region - Ctors -
        public VmsSetupPanelViewModel(ILogService log
                                    , IEventAggregator eventAggregator
                                    , VmsApiSetupViewModel vmsApiSetupViewModel)
                                    :base(eventAggregator)
        {
            _log = log;
            VmsApiSetupViewModel = vmsApiSetupViewModel;

        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            await VmsApiSetupViewModel.ActivateAsync();
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await VmsApiSetupViewModel.DeactivateAsync(true);
            await base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public async void OnActiveTab(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                if (!(args.Source is TabControl)) return;

                if (!(args.AddedItems[0] is TabItem tapItem)) return;

                var contentControl = (tapItem as ContentControl)?.Content as ContentControl;

                if (selectedViewModel != null)
                {
                    await selectedViewModel.DeactivateAsync(true);
                    await Task.Delay(500);
                }

                if (contentControl?.Content is VmsApiSetupView
                    || contentControl is VmsApiSetupView)
                {
                    selectedViewModel = VmsApiSetupViewModel;
                }
                //else if (contentControl?.Content is SurvEventSetupView
                //    || contentControl is SurvEventSetupView)
                //{
                //    selectedViewModel = SurvEventSetupViewModel;
                //}
                //else if (contentControl?.Content is SurvMappingSetupView
                //    || contentControl is SurvMappingSetupView)
                //{
                //    selectedViewModel = SurvMappingSetupViewModel;
                //}
                //else if (contentControl?.Content is SurvSensorSetupView
                //    || contentControl is SurvSensorSetupView)
                //{
                //    selectedViewModel = SurvSensorSetupViewModel;
                //}

                await selectedViewModel.ActivateAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(OnActiveTab)} : {ex.Message}");
            }

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public VmsApiSetupViewModel VmsApiSetupViewModel { get; }
        #endregion
        #region - Attributes -
        private BaseViewModel selectedViewModel;
        private ILogService _log;
        #endregion
    }
}
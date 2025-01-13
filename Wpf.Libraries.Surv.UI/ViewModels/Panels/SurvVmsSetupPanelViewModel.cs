using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Base.Services;
using System.Threading.Tasks;
using System.Threading;
using Wpf.Libraries.Surv.UI.ViewModels.Panels.SetupPanels;
using System.Diagnostics;
using System.Windows.Controls;
using System;
using Wpf.Libraries.Surv.UI.Views.Panels.SetupPanels;

namespace Wpf.Libraries.Surv.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 11/2/2023 10:57:05 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class SurvVmsSetupPanelViewModel : ConductorOneViewModel
    {

        #region - Ctors -
        public SurvVmsSetupPanelViewModel(ILogService log
                                        , IEventAggregator eventAggregator
                                        , SurvApiSetupViewModel survApiSetupViewModel
                                        , SurvEventSetupViewModel survEventSetupViewModel
                                        , SurvMappingSetupViewModel survMappingSetupViewModel
                                        , SurvSensorSetupViewModel survSensorSetupViewModel)
                                        : base(eventAggregator, log)
        {
            SurvApiSetupViewModel = survApiSetupViewModel;
            SurvEventSetupViewModel = survEventSetupViewModel;
            SurvMappingSetupViewModel = survMappingSetupViewModel;
            SurvSensorSetupViewModel = survSensorSetupViewModel;
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            await SurvMappingSetupViewModel.ActivateAsync();
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await SurvApiSetupViewModel.DeactivateAsync(true);
            await SurvEventSetupViewModel.DeactivateAsync(true);
            await SurvMappingSetupViewModel.DeactivateAsync(true);
            await SurvSensorSetupViewModel.DeactivateAsync(true);
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

                if (contentControl?.Content is SurvApiSetupView
                    || contentControl is SurvApiSetupView)
                {
                    selectedViewModel = SurvApiSetupViewModel;
                }
                else if (contentControl?.Content is SurvEventSetupView
                    || contentControl is SurvEventSetupView)
                {
                    selectedViewModel = SurvEventSetupViewModel;
                }
                else if (contentControl?.Content is SurvMappingSetupView
                    || contentControl is SurvMappingSetupView)
                {
                    selectedViewModel = SurvMappingSetupViewModel;
                }
                else if (contentControl?.Content is SurvSensorSetupView
                    || contentControl is SurvSensorSetupView)
                {
                    selectedViewModel = SurvSensorSetupViewModel;
                }

                await selectedViewModel.ActivateAsync();
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(OnActiveTab)} : {ex.Message}");
            }

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public SurvMappingSetupViewModel SurvMappingSetupViewModel { get; }
        public SurvSensorSetupViewModel SurvSensorSetupViewModel { get; }
        public SurvApiSetupViewModel SurvApiSetupViewModel { get; }
        public SurvEventSetupViewModel SurvEventSetupViewModel { get; }
        #endregion
        #region - Attributes -
        private BaseViewModel selectedViewModel;
        #endregion
    }
}

using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Device.UI.Views.Setups;
using Ironwall.Libraries.Devices.Providers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Device.UI.ViewModels.Setups
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/23/2023 6:28:45 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraSetupViewModel : ConductorOneViewModel
    {

        #region - Ctors -
        public CameraSetupViewModel(IEventAggregator eventAggregator
                                    , ILogService log
                                    , CameraMappingSetupViewModel cameraMappingSetupViewModel
                                    , CameraDeviceSetupViewModel cameraDeviceSetupViewModel
                                    , CameraPresetSetupViewModel cameraPresetSetupViewModel
                                    ) :base(eventAggregator, log)
        {
            CameraMappingSetupViewModel = cameraMappingSetupViewModel;
            CameraDeviceSetupViewModel = cameraDeviceSetupViewModel;
            CameraPresetSetupViewModel = cameraPresetSetupViewModel;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            await CameraMappingSetupViewModel.ActivateAsync();
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
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

                // Check if we have a selected TabItem
                if (args.AddedItems.Count == 0 || !(args.AddedItems[0] is TabItem tabItem))
                    return;

                // Extract the content from the selected TabItem
                var tabContent = tabItem.Content as ContentControl;

                //var contentControl = (tapItem as ContentControl)?.Content as ContentControl;

                // Deactivate previous ViewModel if any
                if (selectedViewModel != null)
                    await selectedViewModel.DeactivateAsync(true);

                // If tabContent is a ContentControl, use its Content; otherwise, use tabContent directly.
                var viewContent = (tabContent is ContentControl cc) ? cc.Content : tabContent;

                //if (contentControl?.Content is CameraDeviceSetupView
                //    || contentControl is CameraDeviceSetupView)
                //{
                //    selectedViewModel = CameraDeviceSetupViewModel;
                //}
                //else if (contentControl?.Content is CameraPresetSetupView
                //    || contentControl is CameraPresetSetupView)
                //{
                //    selectedViewModel = CameraPresetSetupViewModel;
                //}
                //else if (contentControl?.Content is CameraMappingSetupView
                //    || contentControl is CameraMappingSetupView)
                //{
                //    selectedViewModel = CameraMappingSetupViewModel;
                //}

                if (viewContent is CameraDeviceSetupView)
                {
                    selectedViewModel = CameraDeviceSetupViewModel;
                }
                else if (viewContent is CameraPresetSetupView)
                {
                    selectedViewModel = CameraPresetSetupViewModel;
                }
                else if (viewContent is CameraMappingSetupView)
                {
                    selectedViewModel = CameraMappingSetupViewModel;
                }
                else 
                {
                    selectedViewModel = null;
                }

                // Activate the selected ViewModel if not null
                if (selectedViewModel != null)
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
        public CameraDeviceSetupViewModel CameraDeviceSetupViewModel { get; private set; }
        public CameraPresetSetupViewModel CameraPresetSetupViewModel { get; private set; }
        public CameraMappingSetupViewModel CameraMappingSetupViewModel { get; private set; }
        #endregion
        #region - Attributes -
        BaseViewModel selectedViewModel;
        private ILogService _log;
        #endregion
    }
}

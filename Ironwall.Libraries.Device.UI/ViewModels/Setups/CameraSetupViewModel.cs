using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Device.UI.Views.Setups;
using Ironwall.Libraries.Devices.Providers;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Controls;
using System;

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
                                    ):base(eventAggregator)
        {

        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            CameraMappingSetupViewModel = IoC.Get<CameraMappingSetupViewModel>();
            CameraDeviceSetupViewModel = IoC.Get<CameraDeviceSetupViewModel>();
            CameraPresetSetupViewModel = IoC.Get<CameraPresetSetupViewModel>();

            NotifyOfPropertyChange(() => CameraMappingSetupViewModel);
            NotifyOfPropertyChange(() => CameraDeviceSetupViewModel);
            NotifyOfPropertyChange(() => CameraPresetSetupViewModel);

            await base.OnActivateAsync(cancellationToken);

            await CameraMappingSetupViewModel.ActivateAsync();
            IsVisible = true;
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
        public async void OnActiveTab(object sender, SelectionChangedEventArgs args)
        {
            try
            {
                if (!(args.Source is TabControl)) return;

                if (!(args.AddedItems[0] is TabItem tapItem)) return;

                var contentControl = (tapItem as ContentControl)?.Content as ContentControl;

                if (selectedViewModel != null)
                    await selectedViewModel.DeactivateAsync(true);

                if (contentControl?.Content is CameraDeviceSetupView
                    || contentControl is CameraDeviceSetupView)
                {
                    selectedViewModel = CameraDeviceSetupViewModel;
                }
                else if (contentControl?.Content is CameraPresetSetupView
                    || contentControl is CameraPresetSetupView)
                {
                    selectedViewModel = CameraPresetSetupViewModel;
                }
                else if (contentControl?.Content is CameraMappingSetupView
                    || contentControl is CameraMappingSetupView)
                {
                    selectedViewModel = CameraMappingSetupViewModel;
                }

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
        public CameraDeviceSetupViewModel CameraDeviceSetupViewModel { get; private set; }
        public CameraPresetSetupViewModel CameraPresetSetupViewModel { get; private set; }
        public CameraMappingSetupViewModel CameraMappingSetupViewModel { get; private set; }
        #endregion
        #region - Attributes -
        BaseViewModel selectedViewModel;
        #endregion
    }
}

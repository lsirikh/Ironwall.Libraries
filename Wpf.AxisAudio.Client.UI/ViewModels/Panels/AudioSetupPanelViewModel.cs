using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System.Threading.Tasks;
using System.Threading;
using System;
using Wpf.AxisAudio.Client.UI.ViewModels.Panels.SetupPanels;
using System.Diagnostics;
using System.Windows.Controls;
using Wpf.AxisAudio.Client.UI.Views.Panels.SetupPanels;
using Ironwall.Libraries.Base.Services;
using Wpf.AxisAudio.Client.UI.Services;

namespace Wpf.AxisAudio.Client.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 10/19/2023 5:35:50 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class AudioSetupPanelViewModel : ConductorOneViewModel
    {

        #region - Ctors -
        public AudioSetupPanelViewModel(IEventAggregator eventAggregator
                                        , ILogService log
                                        , AudioGroupSetupViewModel audioGroupSetupViewModel
                                        , AudioDeviceSetupViewModel audioDeviceSetupViewModel
                                        , AudioSensorSetupViewModel audioSensorSetupViewModel)
                                        : base(eventAggregator)
        {
            AudioGroupSetupViewModel = audioGroupSetupViewModel;
            AudioDeviceSetupViewModel = audioDeviceSetupViewModel;
            AudioSensorSetupViewModel = audioSensorSetupViewModel;
            _log = log;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            await AudioGroupSetupViewModel.ActivateAsync();
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await AudioGroupSetupViewModel.DeactivateAsync(true);
            await AudioDeviceSetupViewModel.DeactivateAsync(true);
            await AudioSensorSetupViewModel.DeactivateAsync(true);
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

                if (contentControl?.Content is AudioGroupSetupView
                    || contentControl is AudioGroupSetupView)
                {
                    selectedViewModel = AudioGroupSetupViewModel;
                }
                else if (contentControl?.Content is AudioDeviceSetupView
                    || contentControl is AudioDeviceSetupView)
                {
                    selectedViewModel = AudioDeviceSetupViewModel;
                }
                else if (contentControl?.Content is AudioSensorSetupView
                    || contentControl is AudioSensorSetupView)
                {
                    selectedViewModel = AudioSensorSetupViewModel;
                }

                await selectedViewModel.ActivateAsync();
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} in {nameof(OnActiveTab)} of {nameof(AudioSetupPanelViewModel)} : {ex}", true);
            }

        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public AudioGroupSetupViewModel AudioGroupSetupViewModel { get; }
        public AudioDeviceSetupViewModel AudioDeviceSetupViewModel { get; }
        public AudioSensorSetupViewModel AudioSensorSetupViewModel { get; }

        private ILogService _log;
        #endregion
        #region - Attributes -
        private BaseViewModel selectedViewModel;
        #endregion
    }
}

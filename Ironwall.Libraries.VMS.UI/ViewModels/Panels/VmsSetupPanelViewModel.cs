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
                                    , VmsApiSetupViewModel vmsApiSetupViewModel
                                    , VmsEventMappingSetupViewModel vmsEventMappingSetupViewModel
                                    , VmsSensorSetupViewModel vmsEventSensorSetupViewModel)
                                    :base(eventAggregator, log)
        {
            VmsApiSetupViewModel = vmsApiSetupViewModel;
            VmsEventMappingSetupViewModel = vmsEventMappingSetupViewModel;
            VmsSensorSetupViewModel = vmsEventSensorSetupViewModel;

        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            await VmsEventMappingSetupViewModel.ActivateAsync();
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            if (selectedViewModel != null)
                await selectedViewModel.DeactivateAsync(true);
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
                // Check if the source is a TabControl
                if (!(args.Source is TabControl)) return;

                // Check if we have a selected TabItem
                if (args.AddedItems.Count == 0 || !(args.AddedItems[0] is TabItem tabItem))
                    return;

                // Extract the content from the selected TabItem
                var tabContent = tabItem.Content as ContentControl;

                //var contentControl = (tapItem as ContentControl)?.Content as ContentControl;

                //if (selectedViewModel != null)
                //{
                //    await selectedViewModel.DeactivateAsync(true);
                //}

                //if (contentControl?.Content is VmsApiSetupView
                //    || contentControl is VmsApiSetupView)
                //{
                //    selectedViewModel = VmsApiSetupViewModel;
                //}
                //else if (contentControl?.Content is VmsEventMappingSetupView
                //    || contentControl is VmsEventMappingSetupView)
                //{
                //    selectedViewModel = VmsEventMappingSetupViewModel;
                //}
                //else if (contentControl?.Content is VmsSensorSetupView
                //    || contentControl is VmsSensorSetupView)
                //{
                //    selectedViewModel = VmsSensorSetupViewModel;
                //}

                //await selectedViewModel.ActivateAsync();

                // Deactivate previous ViewModel if any
                if (selectedViewModel != null)
                {
                    // If a delay is needed, explain why in a comment.
                    // await Task.Delay(500); // Remove or comment out if unnecessary
                    await selectedViewModel.DeactivateAsync(true);
                }

                // If tabContent is a ContentControl, use its Content; otherwise, use tabContent directly.
                var viewContent = (tabContent is ContentControl cc) ? cc.Content : tabContent;

                // Map view types to corresponding ViewModels
                // Note: If a switch expression is desired, consider the complexity.
                if (viewContent is VmsApiSetupView)
                {
                     selectedViewModel = VmsApiSetupViewModel;
                }
                else if (viewContent is VmsEventMappingSetupView)
                {
                    selectedViewModel = VmsEventMappingSetupViewModel;
                }
                else if (viewContent is VmsSensorSetupView)
                {
                    selectedViewModel = VmsSensorSetupViewModel;
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
        public VmsApiSetupViewModel VmsApiSetupViewModel { get; }
        public VmsSensorSetupViewModel VmsSensorSetupViewModel { get; }
        public VmsEventMappingSetupViewModel VmsEventMappingSetupViewModel { get; }
        #endregion
        #region - Attributes -
        private BaseViewModel selectedViewModel;
        private ILogService _log;
        #endregion
    }
}
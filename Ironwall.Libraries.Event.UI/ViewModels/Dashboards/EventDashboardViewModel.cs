using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Events.Services;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Threading;
using System;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Panels;
using System.Diagnostics;
using System.Windows.Controls;
using Ironwall.Libraries.Event.UI.Views.Panels;
using System.Linq;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Event.UI.ViewModels.Dashboards
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/16/2023 9:45:23 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class EventDashboardViewModel : ConductorOneViewModel
    {
        #region - Ctors -
        public EventDashboardViewModel(ILogService log
                                    , IEventAggregator eventAggregator
                                    , DetectionPanelViewModel detectionPanelViewModel
                                    , MalfunctionPanelViewModel malfunctionPanelViewModel
                                    , ActionPanelViewModel actionPanelViewModel
                                    ) : base(eventAggregator)
        {
            _log = log;
            DetectionPanelViewModel = detectionPanelViewModel;
            MalfunctionPanelViewModel = malfunctionPanelViewModel;
            ActionPanelViewModel = actionPanelViewModel;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {

            //await ActivateItemAsync(DetectionPanelViewModel);
            await base.OnActivateAsync(cancellationToken);
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

                if (contentControl?.Content is DetectionPanelView)
                {
                    selectedViewModel = DetectionPanelViewModel;
                }
                else if (contentControl?.Content is MalfunctionPanelView)
                {
                    selectedViewModel = MalfunctionPanelViewModel;
                }
                else if (contentControl?.Content is ActionPanelView)
                {
                    selectedViewModel = ActionPanelViewModel;
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
        public DetectionPanelViewModel DetectionPanelViewModel { get; set; }
        public MalfunctionPanelViewModel MalfunctionPanelViewModel { get; set; }
        public ActionPanelViewModel ActionPanelViewModel { get; set; }

        #endregion
        #region - Attributes -
        private BaseViewModel selectedViewModel;
        private ILogService _log;
        #endregion
    }
}

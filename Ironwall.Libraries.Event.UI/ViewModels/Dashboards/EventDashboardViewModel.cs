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
        public EventDashboardViewModel(IEventAggregator eventAggregator
                                    , ILogService log
                                    , DetectionPanelViewModel detectionPanelViewModel
                                    , MalfunctionPanelViewModel malfunctionPanelViewModel
                                    , ActionPanelViewModel actionPanelViewModel
                                    ) : base(eventAggregator, log)
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
            SelectedIndex = 0; // 첫 탭(Detection) 강제 선택
            await base.OnActivateAsync(cancellationToken);

            selectedViewModel = DetectionPanelViewModel;
            await selectedViewModel.ActivateAsync();
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await base.OnDeactivateAsync(close, cancellationToken);
            await selectedViewModel.DeactivateAsync(true);
            selectedViewModel = null;
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

                ///PREPARING_TIME_MS는 기존의 ViewModel과 View를 바인딩하고, 유연하게 View를 불러오는 시간적 여유를 
                ///제공하므로써, UI/UX의 경험을 보다 우수하게 만들 수 있다. OnActiveTab에서 제어하지 않고, 
                ///각 ViewModel에서 제어를 하게 되면 여러가지 타이밍적 에러에 의해서 버그가 발생하고 해결하기 어려운 과제로 만들게 된다.
                await Task.Delay(PREPARING_TIME_MS);

                if (viewContent is DetectionPanelView)
                {
                    await ActivateItemAsync(DetectionPanelViewModel);
                    selectedViewModel = DetectionPanelViewModel;
                }
                else if (viewContent is MalfunctionPanelView)
                {
                    await ActivateItemAsync(MalfunctionPanelViewModel);
                    selectedViewModel = MalfunctionPanelViewModel;
                }
                else if (viewContent is ActionPanelView)
                {
                    await ActivateItemAsync(ActionPanelViewModel);
                    selectedViewModel = ActionPanelViewModel;
                }
                else
                {
                    selectedViewModel = null;
                }
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
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set { _selectedIndex = value; NotifyOfPropertyChange(() => SelectedIndex); }
        }


        public DetectionPanelViewModel DetectionPanelViewModel { get; set; }
        public MalfunctionPanelViewModel MalfunctionPanelViewModel { get; set; }
        public ActionPanelViewModel ActionPanelViewModel { get; set; }

        #endregion
        #region - Attributes -
        const int PREPARING_TIME_MS = 200;
        private int _selectedIndex;
        private BaseViewModel selectedViewModel;
        #endregion
    }
}

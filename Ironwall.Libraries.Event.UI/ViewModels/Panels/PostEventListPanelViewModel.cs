using Caliburn.Micro;
using Ironwall.Framework.Models.Messages;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using Ironwall.Libraries.Events.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Event.UI.ViewModels.Panels
{
    public class PostEventListPanelViewModel
        : BaseEventListPanelViewModel
    {
        #region - Ctors -
        public PostEventListPanelViewModel(
             IEventAggregator eventAggregator
            , ILogService log
            , PostEventProvider postEventProvider
            , EventSetupModel eventSetupModel)
            : base(eventAggregator, log, eventSetupModel)
        {
            PostEventProvider = postEventProvider;
            CollectionEventViewModel = PostEventProvider.CollectionEntity;

            NameTabHeader = "조치 이벤트";
            NameKind = "EventRangeOutline";
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        void OnClickButtonPrevActionAll(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public int Count => CollectionEventViewModel.Count;
        public ObservableCollection<PostEventViewModel> CollectionEventViewModel { get; set; }
        public PostEventProvider PostEventProvider { get; }
        #endregion
        #region - Attributes -
        #endregion
    }
}

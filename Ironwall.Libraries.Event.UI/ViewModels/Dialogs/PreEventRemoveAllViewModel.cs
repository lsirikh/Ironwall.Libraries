using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Components;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;

namespace Ironwall.Libraries.Event.UI.ViewModels.Dialogs
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 5/12/2023 11:22:45 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class PreEventRemoveAllViewModel : Screen
    {

        #region - Ctors -
        public PreEventRemoveAllViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            IninializeDialog();


            CollectionActionItem[0].IsSelected = true;
            SelectableItemViewModel = CollectionActionItem[0];
        }
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            DeininializeDialog();

            ClearSelection();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        private void IninializeDialog()
        {
            int id = 1;

            CollectionActionItem = new ObservableCollection<SelectableItemViewModel>
            {
                new SelectableItemViewModel(id ++, "야생동물출현"),
                new SelectableItemViewModel(id ++, "강풍/폭우"),
                new SelectableItemViewModel(id ++, "울타리 점검/작업"),
                new SelectableItemViewModel(id ++, "침입발생 특경출동조치"),
                new SelectableItemViewModel(id ++, "오경보"),
            };
            EtcViewModel = new SelectableItemViewModel(id++, "기타");

            EtcViewModel.PropertyChanged += ChangeSelectableItem;

            CollectionActionItem.Apply((item) =>
            {
                item.PropertyChanged += ChangeSelectableItem;
            });
        }
        private void DeininializeDialog()
        {
            EtcViewModel.PropertyChanged -= ChangeSelectableItem;

            CollectionActionItem.Apply((item) =>
            {
                item.PropertyChanged -= ChangeSelectableItem;
            });
        }
        private void ClearSelection()
        {
            foreach (var item in CollectionActionItem)
            {
                item.IsSelected = false;
            }
            EtcViewModel.IsSelected = false;
            SelectableItemViewModel = null;
            Memo = "";
        }

        private void ChangeSelectableItem(object sender, PropertyChangedEventArgs e)
        {
            var vmodel = sender as SelectableItemViewModel;
            if (vmodel.IsSelected)
                SelectableItemViewModel = vmodel;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public string Memo
        {
            get => _memo;
            set
            {
                _memo = value;
                NotifyOfPropertyChange(() => Memo);
            }
        }
        public SelectableItemViewModel EtcViewModel { get; set; }
        public ObservableCollection<SelectableItemViewModel> CollectionActionItem { get; private set; }
        public SelectableItemViewModel SelectableItemViewModel
        {
            get => selectableItemViewModel;
            set
            {
                selectableItemViewModel = value;
                NotifyOfPropertyChange(() => SelectableItemViewModel);
            }
        }
        #endregion
        #region - Attributes -
        private string _memo;
        private SelectableItemViewModel selectableItemViewModel;
        private IEventAggregator _eventAggregator;
        #endregion
    }
}

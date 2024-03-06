using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Event.UI.ViewModels.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ironwall.Libraries.Event.UI.ViewModels.Dialogs
{
    public class PreEventRemoveViewModel : Screen
    {
        #region - Ctors -
        public PreEventRemoveViewModel(IEventAggregator eventAggregator)
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

            if (EventViewModel is PreFaultEventViewModel viewModel)
            {
                Memo = $"{viewModel.TagFault} {viewModel.Type}";
                EtcViewModel.IsSelected = true;
                SelectableItemViewModel = EtcViewModel;
            }
            else
            {
                CollectionActionItem[0].IsSelected = true;
                SelectableItemViewModel = CollectionActionItem[0];
            }
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

        public PreEventViewModel EventViewModel { get; set; }

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

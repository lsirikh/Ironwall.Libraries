using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System;
using System.Windows.Controls;

namespace Ironwall.Framework.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 7/7/2023 9:24:28 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class BaseDataGridViewModel<T> : BaseViewModel
    {
        #region - Ctors -
        public BaseDataGridViewModel()
        {
        }

        public BaseDataGridViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }
        #endregion
        #region - Implementation of Interface -

        #endregion
        #region - Overrides -
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        protected abstract Task CheckSelectState(CancellationToken cancellationToken = default);
        protected abstract Task SelectAll(bool isSelected);
        public abstract void OnClickCheckBoxItem(object sender, RoutedEventArgs e);
        public async void OnClickCheckBoxColumnHeader(object sender, RoutedEventArgs e)
        {
            bool value = false;
            try
            {
                var checkbox = e.Source as CheckBox;
                value = checkbox.IsChecked ?? false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                await SelectAll(value);
            }
        }

        public async Task ClearSelection()
        {
            IsAllChecked = false;
            await SelectAll(false);
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool IsAllChecked
        {
            get { return _isAllChecked; }
            set
            {
                _isAllChecked = value;
                NotifyOfPropertyChange(() => IsAllChecked);
            }
        }

        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);

                if (SelectedItem != null)
                    PropertyUpdate?.Invoke(this, new ValueNotifyEventArgs<T>() { Value = SelectedItem });
            }
        }

        public bool IsCheckedCheckBoxColumnHeader
        {
            get => isCheckedCheckBoxColumnHeader;
            set
            {
                isCheckedCheckBoxColumnHeader = value;
                NotifyOfPropertyChange(() => IsCheckedCheckBoxColumnHeader);
            }
        }

        public int SelectedItemCount
        {
            get { return _selectedItemCount; }
            set
            {
                _selectedItemCount = value;
                NotifyOfPropertyChange(() => SelectedItemCount);
            }
        }
        public IDbConnection DbConnection { get; set; }
        #endregion
        #region - Attributes -
        private bool _isAllChecked;
        private int _selectedItemCount;
        private bool isCheckedCheckBoxColumnHeader;
        private T _selectedItem;
        protected event EventHandler PropertyUpdate;
        protected CancellationTokenSource _pCancellationTokenSource;
        #endregion
    }
}

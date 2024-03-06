using Caliburn.Micro;
using Ironwall.Framework.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ironwall.Framework.ViewModels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 7/7/2023 9:38:00 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class BaseDataGridPanel<T> : BaseDataGridViewModel<T> where T : SelectableBaseViewModel
    {

        #region - Ctors -
        public BaseDataGridPanel()
        {
            ViewModelProvider = new ObservableCollection<T>();
        }
        public BaseDataGridPanel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            ViewModelProvider = new ObservableCollection<T>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            ButtonAllEnable();
            _pCancellationTokenSource = new CancellationTokenSource();
            return base.OnActivateAsync(cancellationToken);
        }
        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await Uninitialize();
            await base.OnDeactivateAsync(close, cancellationToken);
        }

        public abstract void OnClickInsertButton(object sender, RoutedEventArgs e);
        public abstract void OnClickDeleteButton(object sender, RoutedEventArgs e);
        public abstract void OnClickSaveButton(object sender, RoutedEventArgs e);
        public abstract void OnClickReloadButton(object sender, RoutedEventArgs e);

        public override async void OnClickCheckBoxItem(object sender, RoutedEventArgs e)
        {
            try
            {
                var checkbox = e.Source as CheckBox;
                bool value = checkbox.IsChecked ?? false;
                var viewModel = (e.Source as FrameworkElement).DataContext as T;
                viewModel.IsSelected = value;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                await CheckSelectState();
            }
        }

        protected override Task SelectAll(bool isSelected)
        {
            return Task.Run((System.Action)(async () =>
            {
                try
                {
                    foreach (var item in ViewModelProvider)
                    {
                        item.IsSelected = isSelected;
                    }

                    await CheckSelectState();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

            }));
        }

        protected override Task CheckSelectState(CancellationToken cancellationToken = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        SelectedItemCount = ViewModelProvider.Where(entity => entity.IsSelected).Count();
                    }));
                    

                    if (!(SelectedItemCount > 0)) IsAllChecked = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Raised {nameof(Exception)}({nameof(CheckSelectState)}) : {ex.Message}");
                }

            }, cancellationToken);
        }

        protected virtual Task Uninitialize()
        {
            try
            {
                DispatcherService.Invoke((System.Action)(async () =>
                {
                    ButtonAllDisable();
                    await ClearSelection();
                    ViewModelProvider.Clear();
                    SelectedItem = null;
                    SelectedItemCount = 0;
                    IsVisible = false;
                }));

                if (_pCancellationTokenSource != null && !_pCancellationTokenSource.IsCancellationRequested)
                    _pCancellationTokenSource?.Cancel();
                _pCancellationTokenSource?.Dispose();
                
            }
            catch (Exception)
            {
            }
            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        protected void ButtonEnableControl(bool isButton, bool saveButton, bool reloadButton)
        {
            IsButtonEnable = isButton;
            SaveButtonEnable = saveButton;
            ReloadButtonEnable = reloadButton;
            Refresh();
        }
        protected void ButtonAllEnable()
        {
            Debug.WriteLine($"{ClassName}({this.GetHashCode()})의 ButtonAllEnable Start!!");
            IsButtonEnable = true;
            SaveButtonEnable = true;
            ReloadButtonEnable = true;
            Refresh();
            Debug.WriteLine($"{ClassName}({this.GetHashCode()})의 ButtonAllEnable End!!");
        }
        protected void ButtonAllDisable()
        {
            Debug.WriteLine($"{ClassName}({this.GetHashCode()})의 ButtonAllDisable Start!!");
            IsButtonEnable = false;
            SaveButtonEnable = false;
            ReloadButtonEnable = false;
            Debug.WriteLine($"{ClassName}({this.GetHashCode()})의 ButtonAllDisable End!!");
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
                Debug.WriteLine($"{ClassName}({this.GetHashCode()})의 IsVisible:{IsVisible}!!");
            }
        }


        public bool IsButtonEnable
        {
            get { return _isButtonEnable; }
            set 
            { 
                _isButtonEnable = value;
                NotifyOfPropertyChange(() => IsButtonEnable);
            }
        }


        public bool ReloadButtonEnable
        {
            get { return _reloadButtonEnable; }
            set
            {
                _reloadButtonEnable = value;
                NotifyOfPropertyChange(() => ReloadButtonEnable);
            }
        }

        public bool SaveButtonEnable
        {
            get { return _saveButtonEnable; }
            set 
            { 
                _saveButtonEnable = value;
                NotifyOfPropertyChange(() =>  SaveButtonEnable);
            }
        }


        public ObservableCollection<T> ViewModelProvider { get; set; }
        #endregion
        #region - Attributes -
        private bool _isVisible;
        private bool _isButtonEnable;
        private bool _reloadButtonEnable;
        private bool _saveButtonEnable;
        #endregion
    }
}

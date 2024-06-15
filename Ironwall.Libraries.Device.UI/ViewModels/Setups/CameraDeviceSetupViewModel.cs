using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Device.UI.Providers;
using Ironwall.Libraries.Devices.Providers;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.ObjectModel;
using System;
using Ironwall.Framework.Services;
using System.Linq;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Messages;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Devices.Providers.Models;
using Ironwall.Framework.Models;
using Ironwall.Framework.Helpers;
using Ironwall.Libraries.Enums;
using Ironwall.Libraries.Device.UI.Messages;

namespace Ironwall.Libraries.Device.UI.ViewModels.Setups
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/8/2023 5:10:37 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraDeviceSetupViewModel : BaseDataGridPanel<CameraDeviceViewModel>
                                                , IHandle<ResponseCameraInsertMessage>
                                                , IHandle<ResponseCameraReloadMessage>
    {

        #region - Ctors -
        public CameraDeviceSetupViewModel(IEventAggregator eventAggregator)
                                        : base(eventAggregator)
        {
            ViewModelProvider = new ObservableCollection<CameraDeviceViewModel>();
        }

        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);

            _ = DataInitialize(_cancellationTokenSource.Token);

        }

        protected override Task Uninitialize()
        {
            ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
            base.Uninitialize();
            return Task.CompletedTask;
        }
        #endregion
        #region - Binding Methods -
        public void OnClickDiscoveryButton(object sender, RoutedEventArgs e)
        {

        }
        public override async void OnClickInsertButton(object sender, RoutedEventArgs e)
        {
            int id = 0;
            //if(_provider.Count > 0)
            //    id = GetIdHelper.GetCameraId(_provider.LastOrDefault().Id);
            
            //var camera = ModelFactory.Build<CameraDeviceModel>();
            var camera = new CameraDeviceModel();

            var viewModel = ViewModelFactory.Build<CameraDeviceViewModel>(camera);
            await viewModel.ActivateAsync();
            //ViewModelProvider.Add(viewModel);
        }

        public override async void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            foreach (var item in _provider.OfType<CameraDeviceViewModel>().ToList())
            {
                if (item.IsSelected)
                    ViewModelProvider.Remove(item);
            }
            await SelectAll(false);
        }

        public override async void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pCancellationTokenSource.IsCancellationRequested)
                    _pCancellationTokenSource = new CancellationTokenSource();

                await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                ///송신 로직
                await _eventAggregator.PublishOnUIThreadAsync(new RequestCameraInsertMessage());


                await Task.Delay(ACTION_TOKEN_TIMEOUT, _pCancellationTokenSource.Token);
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel(), _pCancellationTokenSource.Token);
            }
            catch (TaskCanceledException ex)
            {
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel());
                Debug.WriteLine($"Rasied {nameof(TaskCanceledException)}({nameof(OnClickSaveButton)} in {ClassName}): {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Rasied {nameof(Exception)}({nameof(OnClickSaveButton)} in {ClassName}): {ex.Message}");
                var explain = ex.Message;

                await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
                {
                    Explain = explain
                }, _pCancellationTokenSource.Token);
            }
        }

        public override async void OnClickReloadButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pCancellationTokenSource.IsCancellationRequested)
                    _pCancellationTokenSource = new CancellationTokenSource();

                await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _pCancellationTokenSource.Token);

                await ClearSelection();
                ///송신 로직
                await _eventAggregator.PublishOnUIThreadAsync(new RequestCameraReloadMessage());

                await Task.Delay(ACTION_TOKEN_TIMEOUT, _pCancellationTokenSource.Token);
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel(), _pCancellationTokenSource.Token);
            }
            catch (TaskCanceledException ex)
            {
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel());
                Debug.WriteLine($"Rasied {nameof(TaskCanceledException)}({nameof(OnClickReloadButton)} in {ClassName}): {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Rasied {nameof(Exception)}({nameof(OnClickReloadButton)} in {ClassName}): {ex.Message}");
                var explain = ex.Message;

                await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
                {
                    Explain = explain
                }, _pCancellationTokenSource.Token);
            }
        }

        #endregion
        #region - Processes -
        private Task DataInitialize(CancellationToken cancellationToken = default)
        {
            return Task.Run(async () =>
            {
                try
                {
                    IsVisible = false;
                    await Task.Delay(1000, cancellationToken);

                    if (cancellationToken.IsCancellationRequested) new TaskCanceledException("Task was cancelled!");
                    //ViewModelProvider Setting
                    _provider = IoC.Get<CameraViewModelProvider>();

                    ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (CameraDeviceViewModel item in _provider)
                        {
                            ViewModelProvider.Add(item);
                        }

                    }));
                    ViewModelProvider.CollectionChanged += ViewModelProvider_CollectionChanged;
                    NotifyOfPropertyChange(() => ViewModelProvider);
                    IsVisible = true;
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Raised {nameof(TaskCanceledException)}({nameof(DataInitialize)}) : {ex.Message}");
                }
                
            });
        }

        private async void ViewModelProvider_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var provider = IoC.Get<DeviceProvider>();
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (CameraDeviceViewModel newItem in e.NewItems)
                    {
                        provider.Add(newItem.Model);
                        _provider.Add(newItem);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (CameraDeviceViewModel newItem in e.OldItems)
                    {
                        provider.Remove(newItem.Model);
                        _provider.Remove(newItem);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (CameraDeviceViewModel oldItem in e.OldItems)
                    {
                        provider.Remove(oldItem.Model);
                        _provider.Remove(oldItem);
                    }
                    foreach (CameraDeviceViewModel newItem in e.NewItems)
                    {
                        provider.Add(newItem.Model);
                        _provider.Add(newItem);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    await provider.ClearData();
                    foreach (CameraDeviceViewModel item in ViewModelProvider)
                    {
                        provider.Add(item.Model);
                    }
                    await provider.Finished();
                    break;
            }
        }

        #endregion
        #region - IHanldes -
        public async Task HandleAsync(ResponseCameraInsertMessage message, CancellationToken cancellationToken)
        {
            _pCancellationTokenSource.Cancel();
            await Task.Delay(500);

            await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
            {
                Explain = $"{message.Message} : {message.IsSuccess}"
            }, _cancellationTokenSource.Token);
        }

        public async Task HandleAsync(ResponseCameraReloadMessage message, CancellationToken cancellationToken)
        {
            _pCancellationTokenSource.Cancel();
            await Task.Delay(500);

            await DataInitialize(_cancellationTokenSource.Token);

            await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
            {
                Explain = $"{message.Message} : {message.IsSuccess}"
            }, _cancellationTokenSource.Token);
        }
        #endregion
        #region - Properties -
        public ObservableCollection<string> CameraComboList
        {
            get { return _cameraComboList; }
            set
            {
                _cameraComboList = value;
                NotifyOfPropertyChange(() => CameraComboList);
            }
        }
        #endregion
        #region - Attributes -
        private ObservableCollection<string> _cameraComboList;
        private CameraViewModelProvider _provider;
        #endregion
    }
}

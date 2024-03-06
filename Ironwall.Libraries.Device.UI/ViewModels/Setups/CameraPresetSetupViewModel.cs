

using Caliburn.Micro;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Device.UI.Messages;
using Ironwall.Libraries.Device.UI.Providers;
using Ironwall.Libraries.Devices.Providers.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.Device.UI.ViewModels.Setups
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/8/2023 5:18:05 PM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraPresetSetupViewModel : BaseDataGridPanel<CameraPresetViewModel>
                                            , IHandle<ResponsePresetInsertMessage>
                                            , IHandle<ResponsePresetReloadMessage>
    {

        #region - Ctors -
        public CameraPresetSetupViewModel(IEventAggregator eventAggregator) 
                                        : base(eventAggregator)
        {
            ViewModelProvider = new ObservableCollection<CameraPresetViewModel>();
            BooleanComboList = new List<bool> { false, true };
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

        public override async void OnClickInsertButton(object sender, RoutedEventArgs e)
        {
            int id = 0;
            if(_provider.Count > 0)
                id = GetIdHelper.GetOptionId(_provider.LastOrDefault().Id);

            var preset = ModelFactory.Build<CameraPresetModel>($"o{id + 1}");

            var viewModel = ViewModelFactory.Build<CameraPresetViewModel>(preset);
            await viewModel.ActivateAsync();
            ViewModelProvider.Add(viewModel);
        }

        public override async void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            foreach (var item in _provider.OfType<CameraPresetViewModel>().ToList())
            {
                if (item.IsSelected)
                    ViewModelProvider.Remove(item);
            }
            await SelectAll(false);
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
                await _eventAggregator.PublishOnUIThreadAsync(new RequestPresetReloadMessage());

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

        public override async void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pCancellationTokenSource.IsCancellationRequested)
                    _pCancellationTokenSource = new CancellationTokenSource();

                await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                ///송신 로직
                await _eventAggregator.PublishOnUIThreadAsync(new RequestPresetInsertMessage());


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
        #endregion
        #region - Binding Methods -
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

                    CameraComboList = (IoC.Get<CameraViewModelProvider>()).Select(entity => entity.Id).ToList();
                    NotifyOfPropertyChange(() => CameraComboList);
                    
                    _provider = IoC.Get<PresetViewModelProvider>();

                    ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (CameraPresetViewModel item in _provider)
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
            var provider = IoC.Get<CameraPresetProvider>();
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (CameraPresetViewModel newItem in e.NewItems)
                    {
                        provider.Add(newItem.Model as CameraPresetModel);
                        _provider.Add(newItem);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (CameraPresetViewModel newItem in e.OldItems)
                    {
                        provider.Remove(newItem.Model as CameraPresetModel);
                        _provider.Remove(newItem);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (CameraPresetViewModel oldItem in e.OldItems)
                    {
                        provider.Remove(oldItem.Model as CameraPresetModel);
                        _provider.Remove(oldItem);
                    }
                    foreach (CameraPresetViewModel newItem in e.NewItems)
                    {
                        provider.Add(newItem.Model as CameraPresetModel);
                        _provider.Add(newItem);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    await provider.ClearData();
                    foreach (CameraPresetViewModel item in ViewModelProvider)
                    {
                        provider.Add(item.Model as CameraPresetModel);
                    }
                    await provider.Finished();
                    break;
            }
        }


        #endregion
        #region - IHanldes -
        public async Task HandleAsync(ResponsePresetReloadMessage message, CancellationToken cancellationToken)
        {
            _pCancellationTokenSource.Cancel();
            await Task.Delay(500);

            await DataInitialize(_cancellationTokenSource.Token);

            await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
            {
                Explain = $"{message.Message} : {message.IsSuccess}"
            }, _cancellationTokenSource.Token);
        }

        public async Task HandleAsync(ResponsePresetInsertMessage message, CancellationToken cancellationToken)
        {
            _pCancellationTokenSource.Cancel();
            await Task.Delay(500);

            await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
            {
                Explain = $"{message.Message} : {message.IsSuccess}"
            }, _cancellationTokenSource.Token);
        }
        #endregion
        #region - Properties -
        public List<string> CameraComboList { get; set; }
        public List<bool> BooleanComboList { get; set; }

        #endregion
        #region - Attributes -
        private PresetViewModelProvider _provider;

        #endregion
    }
}

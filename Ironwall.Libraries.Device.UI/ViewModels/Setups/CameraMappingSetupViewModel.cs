using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Device.UI.Messages;
using Ironwall.Libraries.Device.UI.Providers;
using Ironwall.Libraries.Devices.Providers.Models;
using System;
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
        Created On   : 7/7/2023 8:57:30 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public class CameraMappingSetupViewModel : BaseDataGridPanel<CameraMappingViewModel>
                                                , IHandle<MappingAppliedMessage>
                                                , IHandle<ResponseMappingInsertMessage>
                                                , IHandle<ResponseMappingReloadMessage>
    {

        #region - Ctors -
        public CameraMappingSetupViewModel(IEventAggregator eventAggregator)
                                            : base(eventAggregator)
        {
            ViewModelProvider = new ObservableCollection<CameraMappingViewModel>();
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
            await _eventAggregator.PublishOnUIThreadAsync(new OpenMappingInsertDialog(ViewModelProvider));
        }

        public override void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            DispatcherService.Invoke((System.Action)(async () =>
            {
                foreach (var item in _provider.OfType<CameraMappingViewModel>().ToList())
                {
                    if(item.IsSelected)
                        ViewModelProvider.Remove(item);
                }

                await SelectAll(false);
            }));
        }

        public override async void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pCancellationTokenSource.IsCancellationRequested)
                    _pCancellationTokenSource = new CancellationTokenSource();

                await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);

                ///송신 로직
                await _eventAggregator.PublishOnUIThreadAsync(new RequestMappingInsertMessage());


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
                await _eventAggregator.PublishOnUIThreadAsync(new RequestMappingReloadMessage());


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
                    await Task.Delay(500, cancellationToken);
                    
                    //ViewModelProvider Setting
                    if (cancellationToken.IsCancellationRequested) new TaskCanceledException("Task was cancelled!");

                    _provider = IoC.Get<MappingViewModelProvider>();

                    SensorViewModelProvider = IoC.Get<SensorViewModelProvider>();
                    PresetViewModelProvider = IoC.Get<PresetViewModelProvider>();

                    ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;

                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (CameraMappingViewModel item in _provider)
                        {
                            ViewModelProvider.Add(item);
                        }

                    }));
                    NotifyOfPropertyChange(() => ViewModelProvider);
                    ViewModelProvider.CollectionChanged += ViewModelProvider_CollectionChanged;
                    
                    await Task.Delay(500, cancellationToken);
                    IsVisible = true;
                }
                catch (TaskCanceledException ex)
                {
                    Debug.WriteLine($"Raised {nameof(TaskCanceledException)}({nameof(DataInitialize)} in {ClassName}) : {ex.Message}");
                }
                
            }, cancellationToken);
        }

        private async void ViewModelProvider_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var provider = IoC.Get<CameraMappingProvider>();
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // New items added
                    foreach (CameraMappingViewModel newItem in e.NewItems)
                    {
                        //_provider.Add(newItem);
                        //provider.Add(newItem.Model);
                        await provider.InsertedItem(newItem.Model);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    // Items removed
                    foreach (CameraMappingViewModel newItem in e.OldItems)
                    {
                        //_provider.Remove(newItem);
                        //provider.Remove(newItem.Model);
                        await provider.DeletedItem(newItem.Model);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    // Some items replaced
                    foreach (CameraMappingViewModel oldItem in e.OldItems)
                    {
                        //_provider.Remove(oldItem);
                        //provider.Remove(oldItem.Model);
                        await provider.DeletedItem(oldItem.Model);
                    }
                    foreach (CameraMappingViewModel newItem in e.NewItems)
                    {
                        //_provider.Add(newItem);
                        //provider.Add(newItem.Model);
                        await provider.InsertedItem(newItem.Model);
                    }
                    break;

                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    // The whole list is refreshed
                    //_provider.Clear();
                    //provider.Clear();
                    await provider.ClearData();
                    foreach (CameraMappingViewModel item in ViewModelProvider)
                    {
                        //_provider.Add(item);
                        //provider.Add(item.Model);
                        provider.Add(item.Model);
                    }
                    await provider.Finished();
                    break;
            }
        }

        public async Task HandleAsync(MappingAppliedMessage message, CancellationToken cancellationToken)
        {
            IsVisible = false;
            
            await Task.Delay(500);
            
            ViewModelProvider.CollectionChanged -= ViewModelProvider_CollectionChanged;
            DispatcherService.Invoke((System.Action)(() =>
            {
                ViewModelProvider.Clear();
                foreach (CameraMappingViewModel item in _provider)
                {
                    ViewModelProvider.Add(item);
                }
            }));

            ViewModelProvider.CollectionChanged += ViewModelProvider_CollectionChanged;
            IsVisible = true;

        }

        public async Task HandleAsync(ResponseMappingInsertMessage message, CancellationToken cancellationToken)
        {
            _pCancellationTokenSource.Cancel();
            await Task.Delay(500);

            await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
            {
                Explain = $"{message.Message} : {message.IsSuccess}"
            }, _cancellationTokenSource.Token);
        }

        public async Task HandleAsync(ResponseMappingReloadMessage message, CancellationToken cancellationToken)
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
        #region - IHanldes -
        #endregion
        #region - Properties -
        public SensorViewModelProvider SensorViewModelProvider { get; private set; }
        public PresetViewModelProvider PresetViewModelProvider { get; private set; }
        #endregion
        #region - Attributes -
        public MappingViewModelProvider _provider;
        #endregion
    }
}

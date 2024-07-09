using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.Device.UI.Messages;
using Ironwall.Libraries.Device.UI.Providers;
using Ironwall.Libraries.Devices.Providers;
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
        public CameraMappingSetupViewModel(IEventAggregator eventAggregator
                                        , ILogService log)
                                            : base(eventAggregator)
        {
            ViewModelProvider = new ObservableCollection<CameraMappingViewModel>();
            _log = log;
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

        protected override async Task Uninitialize()
        {
            await base.Uninitialize();
        }

        public override async void OnClickInsertButton(object sender, RoutedEventArgs e)
        {
            await _eventAggregator.PublishOnUIThreadAsync(new OpenMappingInsertDialog(ViewModelProvider));
        }

        public override void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            DispatcherService.Invoke((System.Action)(async () =>
            {
                foreach (var item in ViewModelProvider.ToList())
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

                var list = new List<ICameraMappingModel>();
                foreach (var item in ViewModelProvider)
                {
                    list.Add(item.Model);
                }
                ///송신 로직
                await _eventAggregator.PublishOnUIThreadAsync(new RequestMappingInsertMessage(list));


                await Task.Delay(ACTION_TOKEN_TIMEOUT, _pCancellationTokenSource.Token);
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel(), _pCancellationTokenSource.Token);
            }
            catch (TaskCanceledException ex)
            {
                await _eventAggregator.PublishOnUIThreadAsync(new ClosePopupMessageModel());
                _log.Error($"Rasied {nameof(TaskCanceledException)}({nameof(OnClickSaveButton)} in {ClassName}): {ex.Message}");
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickSaveButton)} in {ClassName}): {ex.Message}");
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
                _log.Error($"Rasied {nameof(TaskCanceledException)}({nameof(OnClickReloadButton)} in {ClassName}): {ex.Message}");
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickReloadButton)} in {ClassName}): {ex.Message}");
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

                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (CameraMappingViewModel item in _provider)
                        {
                            ViewModelProvider.Add(item);
                        }

                    }));
                    NotifyOfPropertyChange(() => ViewModelProvider);
                    
                    await Task.Delay(500, cancellationToken);
                    IsVisible = true;
                }
                catch (TaskCanceledException ex)
                {
                    _log.Error($"Raised {nameof(TaskCanceledException)}({nameof(DataInitialize)} in {ClassName}) : {ex.Message}");
                }
                
            }, cancellationToken);
        }

        public Task HandleAsync(MappingAppliedMessage message, CancellationToken cancellationToken)
        {
            IsVisible = false;
            
            //await Task.Delay(500);
            
            DispatcherService.Invoke((System.Action)(() =>
            {
                foreach (var item in message.Mappings)
                {
                    ViewModelProvider.Add(new CameraMappingViewModel(item));
                }

            }));

            IsVisible = true;

            return Task.CompletedTask;
        }

        public async Task HandleAsync(ResponseMappingInsertMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(500);
            _pCancellationTokenSource.Cancel();

            await DataInitialize(_cancellationTokenSource.Token);

            await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
            {
                Explain = $"{message.Message} : {message.IsSuccess}"
            }, _cancellationTokenSource.Token);
        }

        public async Task HandleAsync(ResponseMappingReloadMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(500);
            _pCancellationTokenSource.Cancel();

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
        private ILogService _log;
        #endregion
    }
}

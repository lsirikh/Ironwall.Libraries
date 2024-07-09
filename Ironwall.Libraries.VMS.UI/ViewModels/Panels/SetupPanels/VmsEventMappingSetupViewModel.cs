using Caliburn.Micro;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.UI.Messages;
using Ironwall.Libraries.VMS.UI.Providers.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Ironwall.Libraries.Enums;
using Sensorway.Events.Base.Models;

namespace Ironwall.Libraries.VMS.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/13/2024 3:18:42 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsEventMappingSetupViewModel : BaseDataGridPanel<VmsMappingViewModel>
                                        , IHandle<ResponseApiEventListMessage>
                                        , IHandle<ResponseApiMappingInsertMessage>
                                        , IHandle<ResponseApiMappingReloadMessage>
    {
        #region - Ctors -
        public VmsEventMappingSetupViewModel(ILogService log
                                            , IEventAggregator eventAggregator)
                                            : base(eventAggregator)
        {
            _log = log;
            ViewModelProvider = new ObservableCollection<VmsMappingViewModel>();
            VmsEvents = new ObservableCollection<IEventModel>();
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
            await _eventAggregator.PublishOnUIThreadAsync(new RequestApiEventListMessage());
            _ = DataInitialize(_cancellationTokenSource.Token);
        }

        protected override async Task Uninitialize()
        {
            await base.Uninitialize();
        }

        public override void OnClickDeleteButton(object sender, RoutedEventArgs e)
        {
            try
            {
                DispatcherService.Invoke((System.Action)(async () =>
                {
                    foreach (var item in ViewModelProvider.ToList())
                    {
                        if (item.IsSelected)
                            ViewModelProvider.Remove(item);
                    }

                    await SelectAll(false);
                }));
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickDeleteButton)} in {ClassName}): {ex.Message}");

            }
        }

        public override async void OnClickInsertButton(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = 0;
                if (ViewModelProvider.Count > 0)
                    id = ViewModelProvider.Max(entity => entity.Id);

                var model = new VmsMappingModel(0, 0, 0, EnumTrueFalse.True);

                var viewModel = new VmsMappingViewModel(model);
                await viewModel.ActivateAsync();
                ViewModelProvider.Add(viewModel);
            }
            catch (Exception ex)
            {
                _log.Error($"Rasied {nameof(Exception)}({nameof(OnClickInsertButton)} in {ClassName}): {ex.Message}");
            }
        }

        public override async void OnClickSaveButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pCancellationTokenSource.IsCancellationRequested)
                    _pCancellationTokenSource = new CancellationTokenSource();

                await _eventAggregator.PublishOnUIThreadAsync(new OpenProgressPopupMessageModel(), _cancellationTokenSource.Token);
                var list = new List<IVmsMappingModel>();
                foreach (var item in ViewModelProvider)
                {
                    list.Add(item.Model);
                }
                ///송신 로직
                await _eventAggregator.PublishOnUIThreadAsync(new RequestApiMappingInsertMessage(list));


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
                await _eventAggregator.PublishOnUIThreadAsync(new RequestApiMappingReloadMessage());


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

                    //ViewModelProvider Setting
                    if (cancellationToken.IsCancellationRequested) new TaskCanceledException("Task was cancelled!");

                    _provider = IoC.Get<VmsMappingViewModelProvider>();

                    DispatcherService.Invoke((System.Action)(() =>
                    {
                        ViewModelProvider.Clear();
                        foreach (var item in _provider)
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
                    _log.Error($"Raised {nameof(TaskCanceledException)}({nameof(DataInitialize)}) : {ex.Message}");
                }
            });
        }

        #endregion
        #region - IHanldes -
        public Task HandleAsync(ResponseApiMappingInsertMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(ResponseApiMappingReloadMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task HandleAsync(ResponseApiEventListMessage message, CancellationToken cancellationToken)
        {
            await Task.Delay(500);
            if (_pCancellationTokenSource == null && _pCancellationTokenSource.IsCancellationRequested)
                _pCancellationTokenSource = new CancellationTokenSource();
            
            _pCancellationTokenSource.Cancel();

            VmsEvents.Clear();
            foreach (var item in message.Events)
            {
                VmsEvents.Add(item);
            }
            NotifyOfPropertyChange(() => VmsEvents);    

            await _eventAggregator.PublishOnUIThreadAsync(new OpenInfoPopupMessageModel
            {
                Explain = $"{message.Message} : {message.IsSuccess}"
            }, _cancellationTokenSource.Token);
        }
        #endregion
        #region - Properties -
        public ObservableCollection<IEventModel> VmsEvents { get; private set; }
        #endregion
        #region - Attributes -
        private ILogService _log;
        private VmsMappingViewModelProvider _provider;
        #endregion

    }
}
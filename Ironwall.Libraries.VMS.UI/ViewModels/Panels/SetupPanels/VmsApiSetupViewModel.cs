using Caliburn.Micro;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.Services;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using Ironwall.Libraries.VMS.Common.Providers.Models;
using Ironwall.Libraries.VMS.UI.Messages;
using Ironwall.Libraries.VMS.UI.Providers.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Ironwall.Libraries.VMS.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/12/2024 4:12:56 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsApiSetupViewModel : BaseDataGridPanel<VmsApiViewModel>
                                        , IHandle<ResponseApiSettingInsertMessage>
                                        , IHandle<ResponseApiSettingReloadMessage>
    {
        #region - Ctors -
        public VmsApiSetupViewModel(ILogService log
                                    , IEventAggregator eventAggregator) 
                                    : base(eventAggregator)
        {
            _log = log;

            ViewModelProvider = new ObservableCollection<VmsApiViewModel>();
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

                var model = new VmsApiModel(0, "", (uint)80, "", "");

                var viewModel = new VmsApiViewModel(model);
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

                var list = new List<IVmsApiModel>();
                foreach (var item in ViewModelProvider)
                {
                    list.Add(item.Model);
                }
                ///송신 로직
                await _eventAggregator.PublishOnUIThreadAsync(new RequestApiSettingInsertMessage(list));


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
                await _eventAggregator.PublishOnUIThreadAsync(new RequestApiSettingReloadMessage());

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

                    _provider = IoC.Get<VmsApiViewModelProvider>();

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
        public Task HandleAsync(ResponseApiSettingInsertMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(ResponseApiSettingReloadMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private VmsApiViewModelProvider _provider;
        #endregion

    }
}
using Caliburn.Micro;
using Ironwall.Framework.ViewModels;
using Ironwall.Libraries.Base.Services;
using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using Ironwall.Framework.Models.Vms;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Services;
using Ironwall.Libraries.VMS.UI.Messages;
using Ironwall.Libraries.VMS.UI.Providers.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.VMS.UI.ViewModels.Panels.SetupPanels
{
    /****************************************************************************
       Purpose      :                                                          
       Created By   : GHLee                                                
       Created On   : 6/11/2024 2:58:45 PM                                                    
       Department   : SW Team                                                   
       Company      : Sensorway Co., Ltd.                                       
       Email        : lsirikh@naver.com                                         
    ****************************************************************************/
    public class VmsSensorSetupViewModel : BaseDataGridPanel<VmsSensorViewModel>
                                        , IHandle<ResponseApiSensorInsertMessage>
                                        , IHandle<ResponseApiSensorReloadMessage>
    {
        #region - Ctors -
        public VmsSensorSetupViewModel(ILogService log
                                    , IEventAggregator eventAggregator)
                                    : base(eventAggregator)
        {
            _log = log;

            ViewModelProvider = new ObservableCollection<VmsSensorViewModel>();
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

                var model = new VmsSensorModel(0, 0, null, EnumTrueFalse.True);

                var viewModel = new VmsSensorViewModel(model);
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

                var list = new List<IVmsSensorModel>();
                foreach (var item in ViewModelProvider)
                {
                    list.Add(item.Model);
                }
                ///송신 로직
                await _eventAggregator.PublishOnUIThreadAsync(new RequestApiSensorInsertMessage(list));


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
                await _eventAggregator.PublishOnUIThreadAsync(new RequestApiSensorReloadMessage());


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

                    _provider = IoC.Get<VmsSensorViewModelProvider>();

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
        public Task HandleAsync(ResponseApiSensorInsertMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task HandleAsync(ResponseApiSensorReloadMessage message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion
        #region - Properties -
        #endregion
        #region - Attributes -
        private ILogService _log;
        private VmsSensorViewModelProvider _provider;
        #endregion

    }
}
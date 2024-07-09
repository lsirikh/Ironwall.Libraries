using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using Ironwall.Libraries.Events.Services;
using System.Threading.Tasks;
using System.Threading;
using System;
using ControlzEx.Standard;
using System.Diagnostics;
using Ironwall.Libraries.Event.UI.Models.Messages;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Enums;
using System.Linq;
using Ironwall.Framework.ViewModels;
using System.Collections.ObjectModel;
using Ironwall.Libraries.Base.Services;

namespace Ironwall.Libraries.Event.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/16/2023 10:24:29 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public sealed class ActionPanelViewModel : EventBasePanelViewModel<IActionEventModel>
                                             , IHandle<SearchEventListMessageModel<IActionEventModel>>
    {

        #region - Ctors -
        public ActionPanelViewModel(ILogService log
                                    , IEventAggregator eventAggregator
                                    ) : base(eventAggregator)
        {
            _log = log;
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override Task EventInitialize()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (_cancellationTokenSource != null)
                        _cancellationTokenSource.Cancel();

                    _cancellationTokenSource = new CancellationTokenSource();

                    IsVisible = false;

                    await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), EndDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), Enums.EnumEventType.Action));

                    await Task.Delay(ACTION_TOKEN_TIMEOUT, _cancellationTokenSource.Token);
                    IsVisible = true;
                }
                catch (NotSupportedException)
                {
                }
                catch (Exception)
                {
                    throw;
                }

            });
        }

        protected override void EventClear()
        {
            ViewModelProvider?.Clear();
            NotifyOfPropertyChange(() => ViewModelProvider);
            Total = 0;
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -
        public bool CanClckSearch => true;
        public async void ClickSearch()
        {
            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                IsVisible = false;

                await Task.Delay(500, _cancellationTokenSource.Token);
                await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), EndDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), Enums.EnumEventType.Action));
                await Task.Delay(ACTION_TOKEN_TIMEOUT, _cancellationTokenSource.Token);

                IsVisible = true;
            }
            catch (TaskCanceledException ex)
            {
                _log.Error(ex.Message);
                IsVisible = true;
            }
            catch (Exception ex)
            {
                _log.Error($"Raised Exception in {nameof(ClickSearch)}({nameof(ActionPanelViewModel)}) : " + ex.Message);
            }
        }
        public bool CanClickCancel => true;
        public void ClickCancel()
        {
            try
            {
                if (_cancellationTokenSource == null && _cancellationTokenSource.IsCancellationRequested)
                    return;

                _cancellationTokenSource.Cancel();
            }
            catch
            {
            }

        }
        #endregion
        #region - IHanldes -
        public Task HandleAsync(SearchEventListMessageModel<IActionEventModel> message, CancellationToken cancellationToken)
        {
            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                ViewModelProvider = new ObservableCollection<IActionEventModel>(message.Lists);
                NotifyOfPropertyChange(() => ViewModelProvider);

                Total = ViewModelProvider.Count();
                IsVisible = true;


                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #region - Properties -
        public ActionViewModelProvider ActionViewModelProvider { get; private set; }
        #endregion
        #region - Attributes -
        private ActionEventProvider _actionProvider;
        private ILogService _log;
        #endregion
    }
}

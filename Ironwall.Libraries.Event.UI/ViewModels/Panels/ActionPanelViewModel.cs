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

    public sealed class ActionPanelViewModel : EventBasePanelViewModel
         , IHandle<SearchEventResultMessageModel>
    {

        #region - Ctors -
        public ActionPanelViewModel(IEventAggregator eventAggregator
                                    ) : base(eventAggregator)
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
                await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString(), EndDate.ToString(), Enums.EnumEventType.Action));
                await Task.Delay(ACTION_TOKEN_TIMEOUT, _cancellationTokenSource.Token);

                IsVisible = true;
            }
            catch (TaskCanceledException ex)
            {
                Debug.WriteLine(ex.Message);
                IsVisible = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(ClickSearch)}({nameof(ActionPanelViewModel)}) : " + ex.Message);
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

        protected override Task EventInitialize()
        {
            return Task.Run(() =>
            {
                try
                {
                    _actionProvider = IoC.Get<ActionEventProvider>();
                    ActionViewModelProvider = new ActionViewModelProvider(_actionProvider);
                    NotifyOfPropertyChange(() => ActionViewModelProvider);
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
            ActionViewModelProvider?.Clear();
            NotifyOfPropertyChange(() => ActionViewModelProvider);
        }

        #endregion
        #region - IHanldes -
        public async Task HandleAsync(SearchEventResultMessageModel message, CancellationToken cancellationToken)
        {
            try
            {

                if (!(message.Model is ISearchActionReponseModel response))
                    throw new NullReferenceException(message: "Casted ResponseModel was null...");

                await _eventProvider?.ClearData();
                await _actionProvider?.ClearData();
                ActionViewModelProvider?.Uninitialize();

                await CreateDetectionEvent(response.DetectionEvents);
                await CreateMalfunctionEvent(response.MalfunctionEvents);

                var deviceProvider = IoC.Get<DeviceProvider>();
                foreach (IActionRequestModel item in response.ActionEvents.OrderBy(entity=>entity.DateTime).ToList())
                {
                    var eventModel = _eventProvider.Where(e => e.Id == item.EventId).FirstOrDefault();
                    
                    if (eventModel == null) continue;

                    var actionModel = ModelFactory.Build<ActionEventModel>(item, eventModel);
                    _actionProvider.Add(actionModel);
                }

                ActionViewModelProvider = new ActionViewModelProvider(_actionProvider);
                await ActionViewModelProvider.Initialize();
                NotifyOfPropertyChange(() => ActionViewModelProvider);
                Total = ActionViewModelProvider.Count();

                _cancellationTokenSource.Cancel();
                IsVisible = true;
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine($"Rasied Exception in {nameof(HandleAsync)} for {nameof(SearchEventResultMessageModel)} : {ex.Message}");
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
        #endregion
    }
}

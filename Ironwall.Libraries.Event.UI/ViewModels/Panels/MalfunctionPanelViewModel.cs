using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using Ironwall.Libraries.Events.Services;
using System.Threading.Tasks;
using System.Threading;
using System;
using ControlzEx.Standard;
using System.Diagnostics;
using Ironwall.Libraries.Devices.Providers;
using Ironwall.Libraries.Event.UI.Models.Messages;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models;
using System.Linq;
using Ironwall.Libraries.Enums;

namespace Ironwall.Libraries.Event.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/16/2023 10:22:59 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public sealed class MalfunctionPanelViewModel : EventBasePanelViewModel
        , IHandle<SearchEventResultMessageModel>
    {

        #region - Ctors -
        public MalfunctionPanelViewModel(IEventAggregator eventAggregator
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
                await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString(), EndDate.ToString(), Enums.EnumEventType.Fault));
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
                Debug.WriteLine($"Raised Exception in {nameof(ClickSearch)}({nameof(MalfunctionPanelViewModel)}) : " + ex.Message);
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
                    MalfunctionViewModelProvider = new MalfunctionViewModelProvider(_eventProvider);
                    NotifyOfPropertyChange(() => MalfunctionViewModelProvider);
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
            MalfunctionViewModelProvider?.Clear();
            NotifyOfPropertyChange(() => MalfunctionViewModelProvider);
            Total = 0;
            Reported = 0;
        }

        #endregion
        #region - IHanldes -
        public async Task HandleAsync(SearchEventResultMessageModel message, CancellationToken cancellationToken)
        {
            try
            {
                if (!(message.Model is ISearchMalfunctionResponseModel response))
                    throw new NullReferenceException(message: "Casted ResponseModel was null...");

                await _eventProvider?.ClearData();
                MalfunctionViewModelProvider?.Uninitialize();

                await CreateMalfunctionEvent(response.Events);

                MalfunctionViewModelProvider = new MalfunctionViewModelProvider(_eventProvider);
                await MalfunctionViewModelProvider.Initialize();
                NotifyOfPropertyChange(() => MalfunctionViewModelProvider);
                
                Total = MalfunctionViewModelProvider.Count();
                Reported = MalfunctionViewModelProvider.Where(entity => entity.Status == 1).Count();
                NotifyOfPropertyChange(() => UnReported);

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

        public int Reported
        {
            get { return _reported; }
            set
            {
                _reported = value;
                NotifyOfPropertyChange(() => Reported);
                NotifyOfPropertyChange(() => UnReported);
            }
        }

        public int UnReported => Total - Reported;
        public MalfunctionViewModelProvider MalfunctionViewModelProvider { get; private set; }

        #endregion
        #region - Attributes -
        private int _reported;
        #endregion
    }
}

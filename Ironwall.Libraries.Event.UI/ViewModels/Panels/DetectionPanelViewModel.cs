using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using Ironwall.Libraries.Events.Services;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Threading;
using System;
using Ironwall.Libraries.Event.UI.Providers.ViewModels;
using ControlzEx.Standard;
using System.Diagnostics;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Framework.Models.Messages;
using Ironwall.Framework.Models.Communications.Accounts;
using Ironwall.Framework.Models.Communications;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Libraries.Events.Models;
using Ironwall.Libraries.Event.UI.Models.Messages;
using Ironwall.Framework.Models;
using Ironwall.Framework.Models.Events;
using Ironwall.Libraries.Devices.Providers;
using System.Linq;
using Ironwall.Framework.Models.Devices;


namespace Ironwall.Libraries.Event.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/16/2023 10:22:43 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public sealed class DetectionPanelViewModel : EventBasePanelViewModel
        , IHandle<SearchEventResultMessageModel>

    {

        #region - Ctors -
        public DetectionPanelViewModel(IEventAggregator eventAggregator
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
                await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString(), EndDate.ToString(), Enums.EnumEventType.Intrusion));
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
                Debug.WriteLine($"Raised Exception in {nameof(ClickSearch)}({nameof(DetectionPanelViewModel)}) : " + ex.Message);
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
                    _eventProvider = IoC.Get<EventProvider>();
                    
                    DetectionViewModelProvider = new DetectionViewModelProvider(_eventProvider);
                    NotifyOfPropertyChange(() => DetectionViewModelProvider);
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
            DetectionViewModelProvider?.Clear();
            NotifyOfPropertyChange(() => DetectionViewModelProvider);
            Total = 0;
            Reported = 0;
        }


        #endregion
        #region - IHanldes -
        public async Task HandleAsync(SearchEventResultMessageModel message, CancellationToken cancellationToken)
        {

            try
            {
                if(!(message.Model is ISearchDetectionResponseModel response))
                    throw new NullReferenceException(message: "Casted ResponseModel was null...");

                await _eventProvider?.ClearData();
                DetectionViewModelProvider?.Uninitialize();

                await CreateDetectionEvent(response.Events);

                DetectionViewModelProvider = new DetectionViewModelProvider(_eventProvider);
                await DetectionViewModelProvider.Initialize();
                NotifyOfPropertyChange(() => DetectionViewModelProvider);

                Total = DetectionViewModelProvider.Count();
                Reported = DetectionViewModelProvider.Where(entity => entity.Status == 1).Count();
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
        public DetectionViewModelProvider DetectionViewModelProvider { get; private set; }
        #endregion
        #region - Attributes -
        private int _reported;
        #endregion
    }
}

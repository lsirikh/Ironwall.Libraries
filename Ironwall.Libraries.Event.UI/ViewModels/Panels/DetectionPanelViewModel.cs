﻿using Caliburn.Micro;
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
using Ironwall.Libraries.Enums;
using System.Collections.ObjectModel;
using Ironwall.Libraries.Base.Services;


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

    public sealed class DetectionPanelViewModel : EventBasePanelViewModel<IDetectionEventModel>
                                                , IHandle<SearchEventListMessageModel<IDetectionEventModel>>
    {

        #region - Ctors -
        public DetectionPanelViewModel(ILogService log
                                        , IEventAggregator eventAggregator)
                                        : base(eventAggregator, log)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected override async Task EventInitialize()
        {
            try
            {
                IsVisible = false;

                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                _cancellationTokenSource = new CancellationTokenSource();

                IsVisible = false;

                await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), EndDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), Enums.EnumEventType.Intrusion));

                await Task.Delay(ACTION_TOKEN_TIMEOUT, _cancellationTokenSource.Token);

                IsVisible = true;
            }
            catch (NotSupportedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} for {ex.Message}");
            }
            finally
            {
                IsVisible = true;
            }
        }

        protected override void EventClear()
        {
            ViewModelProvider?.Clear();
            NotifyOfPropertyChange(() => ViewModelProvider);
            Total = 0;
            Reported = 0;
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

                await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), EndDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), Enums.EnumEventType.Intrusion));
                await Task.Delay(ACTION_TOKEN_TIMEOUT, _cancellationTokenSource.Token);
                
                IsVisible = true;
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                _log.Error($"Raised {nameof(Exception)} for {ex.Message}");
            }
            finally
            {
                IsVisible = true;
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
        public Task HandleAsync(SearchEventListMessageModel<IDetectionEventModel> message, CancellationToken cancellationToken)
        {
            try
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                    _cancellationTokenSource.Cancel();

                ViewModelProvider = new ObservableCollection<IDetectionEventModel>(message.Lists);
                NotifyOfPropertyChange(() => ViewModelProvider);

                Total = ViewModelProvider.Count();
                Reported = ViewModelProvider.Where(entity => entity.Status == EnumTrueFalse.True).Count();
                NotifyOfPropertyChange(() => UnReported);
                IsVisible = true;
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsVisible = true;
            }

            return Task.CompletedTask;
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

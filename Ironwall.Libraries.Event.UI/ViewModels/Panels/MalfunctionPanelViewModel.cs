﻿using Caliburn.Micro;
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
using Ironwall.Libraries.Event.UI.ViewModels.Events;
using Ironwall.Libraries.Base.Services;
using System.Collections.ObjectModel;

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

    public sealed class MalfunctionPanelViewModel : EventBasePanelViewModel<IMalfunctionEventModel>
                                                , IHandle<SearchEventListMessageModel<IMalfunctionEventModel>>
    {

        #region - Ctors -
        public MalfunctionPanelViewModel(IEventAggregator eventAggregator
                                        , ILogService log
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

                    await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), EndDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), Enums.EnumEventType.Fault));

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

                await Task.Delay(500, _cancellationTokenSource.Token);
                await _eventAggregator.PublishOnUIThreadAsync(new SearchEventMessageModel(StartDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), EndDate.ToString("yyyy-MM-dd HH:mm:ss.ff"), Enums.EnumEventType.Fault));
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
                _log.Error($"Raised Exception in {nameof(ClickSearch)}({nameof(MalfunctionPanelViewModel)}) : " + ex.Message);
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
        public Task HandleAsync(SearchEventListMessageModel<IMalfunctionEventModel> message, CancellationToken cancellationToken)
        {
            try
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Cancel();

                ViewModelProvider = new ObservableCollection<IMalfunctionEventModel>(message.Lists);
                NotifyOfPropertyChange(() => ViewModelProvider);

                Total = ViewModelProvider.Count();
                Reported = ViewModelProvider.Where(entity => entity.Status == EnumTrueFalse.True).Count();
                NotifyOfPropertyChange(() => UnReported);
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
        private ILogService _log;
        #endregion
    }
}

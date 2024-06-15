using Caliburn.Micro;
using Ironwall.Framework.ViewModels.ConductorViewModels;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Threading;
using System;
using Ironwall.Libraries.Events.Providers;
using Ironwall.Framework.Models.Communications.Events;
using Ironwall.Framework.Models.Devices;
using Ironwall.Framework.Models.Events;
using Ironwall.Framework.Models;
using Ironwall.Libraries.Devices.Providers;
using System.Linq;
using System.Collections.Generic;
using Ironwall.Libraries.Enums;
using System.Diagnostics;
using System.Diagnostics.Eventing;
using EventProvider = Ironwall.Libraries.Events.Providers.EventProvider;
using Ironwall.Framework.Helpers;
using Ironwall.Framework.ViewModels;

namespace Ironwall.Libraries.Event.UI.ViewModels.Panels
{
    /****************************************************************************
        Purpose      :                                                           
        Created By   : GHLee                                                
        Created On   : 6/21/2023 10:35:05 AM                                                    
        Department   : SW Team                                                   
        Company      : Sensorway Co., Ltd.                                       
        Email        : lsirikh@naver.com                                         
     ****************************************************************************/

    public abstract class EventBasePanelViewModel : BaseViewModel
    {

        #region - Ctors -
        public EventBasePanelViewModel(IEventAggregator eventAggregator
                                    ) : base(eventAggregator)
        {
        }
        #endregion
        #region - Implementation of Interface -
        #endregion
        #region - Overrides -
        protected abstract Task EventInitialize();
        protected abstract void EventClear();
        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            StartDate = DateTimeHelper.GetCurrentTimeWithoutMS() - TimeSpan.FromDays(1);
            EndDate = DateTimeHelper.GetCurrentTimeWithoutMS();
            EndDateDisplay = StartDate;

            IsVisible = true;

            _eventProvider = IoC.Get<EventProvider>();
            await EventInitialize();

            await base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            EventClear();
            return base.OnDeactivateAsync(close, cancellationToken);
        }
        #endregion
        #region - Binding Methods -
        #endregion
        #region - Processes -

        public virtual Task CreateDetectionEvent(List<DetectionRequestModel> events, CancellationToken cancellationToken = default)
        {
            try
            {
                var deviceProvider = IoC.Get<DeviceProvider>();
                foreach (IDetectionRequestModel item in events)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();

                    var device = deviceProvider.OfType<ISensorDeviceModel>()
                        .Where(entity =>
                        entity.Controller.DeviceNumber == item.Controller
                        && entity.DeviceNumber == item.Sensor
                        && entity.DeviceType == item.UnitType
                        ).FirstOrDefault();
                    var model = ModelFactory.Build<DetectionEventModel>(item, device);
                    _eventProvider.Add(model);
                }
            }
            catch(NullReferenceException ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(CreateDetectionEvent)} : " + ex.Message);
            }
            catch (Exception)
            {
            }
            return Task.CompletedTask;
        }

        public virtual Task CreateMalfunctionEvent(List<MalfunctionRequestModel> events, CancellationToken cancellationToken = default)
        {
            try
            {
                var deviceProvider = IoC.Get<DeviceProvider>();
                foreach (IMalfunctionRequestModel item in events)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new TaskCanceledException();

                    switch ((EnumDeviceType)item.UnitType)
                    {
                        case EnumDeviceType.NONE:
                            break;
                        case EnumDeviceType.Controller:
                            {
                                var device = deviceProvider.OfType<IControllerDeviceModel>()
                                                            .Where(entity =>
                                                            entity.DeviceNumber == item.Controller
                                                            && entity.DeviceType == item.UnitType
                                                            ).FirstOrDefault();
                                var model = ModelFactory.Build<MalfunctionEventModel>(item, device);
                                _eventProvider.Add(model);
                            }
                            break;
                        case EnumDeviceType.Multi:
                        case EnumDeviceType.Fence:
                        case EnumDeviceType.Underground:
                        case EnumDeviceType.Contact:
                        case EnumDeviceType.PIR:
                        case EnumDeviceType.IoController:
                        case EnumDeviceType.Laser:
                            {
                                var device = deviceProvider.OfType<ISensorDeviceModel>()
                                                            .Where(entity =>
                                                            entity.Controller.DeviceNumber == item.Controller
                                                            && entity.DeviceNumber == item.Sensor
                                                            && entity.DeviceType == item.UnitType
                                                            ).FirstOrDefault();
                                var model = ModelFactory.Build<MalfunctionEventModel>(item, device);
                                _eventProvider.Add(model);
                            }
                            break;
                        case EnumDeviceType.Cable:
                            break;
                        case EnumDeviceType.IpCamera:
                            break;
                        case EnumDeviceType.Fence_Line:
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                Debug.WriteLine($"Raised Exception in {nameof(CreateMalfunctionEvent)} : " + ex.Message);
            }
            catch (Exception)
            {
            }
            return Task.CompletedTask;
        }
        #endregion
        #region - IHanldes -
        #endregion
        #region - Properties -
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                _startDate = value;
                NotifyOfPropertyChange(() => StartDate);
                EndDateDisplay = _startDate;
            }
        }

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                NotifyOfPropertyChange(() => EndDate);
            }
        }

        public DateTime EndDateDisplay
        {
            get { return _endDateDisplay; }
            set
            {
                _endDateDisplay = value;
                NotifyOfPropertyChange(() => EndDateDisplay);
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyOfPropertyChange(() => IsVisible);
            }
        }


        public int Total
        {
            get { return _total; }
            set
            {
                _total = value;
                NotifyOfPropertyChange(() => Total);
            }
        }

        #endregion
        #region - Attributes -
        protected DateTime _startDate;
        protected DateTime _endDate;
        protected DateTime _endDateDisplay;
        protected int _total;
        protected bool _isVisible;
        protected EventProvider _eventProvider;
        #endregion
    }
}
